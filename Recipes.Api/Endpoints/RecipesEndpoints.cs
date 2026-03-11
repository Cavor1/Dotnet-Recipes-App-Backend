using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Recipes.Api.Data;
using Recipes.Api.Dto;
using Recipes.Api.Entities;
using Recipes.Api.Validation;

namespace Recipes.Api.Endpoints;

public static class RecipesEndpoints
{
    public static void MapRecipesEndpoints(this WebApplication app)
    {
        app.MapGet("/recipes", GetRecipes);
        app.MapGet("/recipes/{id:guid}", GetRecipe);
        app.MapPost("/recipes", CreateRecipe);
    //     app.MapPut("/recipes/{id:guid}", UpdateRecipe);
    //     app.MapDelete("/recipes/{id:guid}", DeleteRecipe);
    }
    static async Task<IResult> GetRecipes(AppDbContext db)
    {
        var recipes = await db.Recipes.Select(r => new RecipeDto()
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description //?might be bad idea 
        }).ToListAsync();
        return Results.Ok(recipes);
    }
    static async Task<IResult> GetRecipe(Guid id,AppDbContext db)
    {
        var recipe = await db.Recipes.FindAsync(id);
        return recipe is null
            ? Results.NotFound()
            : Results.Ok(new RecipeDto()
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description
            });
    }

    static async Task<IResult> CreateRecipe(CreateRecipeDto req, AppDbContext db)
    {
    
        var reqValidation = req.Validate();
        if (reqValidation is not null) return reqValidation;

        foreach (var ri in req.RecipeIngredients)
        {
            var riValidation = ri.Validate();
            if (riValidation is not null) return riValidation;
        }

        var reqIngredients = req.RecipeIngredients.Select(r => new
        {
            Name = r.Name,
            Quantity = r.Quantity
        });

        var existingIngredients = await db.Ingredients
            .Where(ei => req.RecipeIngredients
                .Select(ri => ri.Name.Trim().ToLower())
                .Contains(ei.Name))
            .ToDictionaryAsync(i => i.Name);

        //get existing ingredients
        //create and add nonexistent
        //create recipeingredients and add to database

        var Ingredient = new Ingredient()
        {
            Id = Guid.NewGuid(),
            Name = "test",

        };

        var recipe = new Recipe()
        {
            Id = Guid.NewGuid(),
            Title = req.Title,
            Description = req.Description
        };

     



        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();

        return Results.Created($"/recipes/{recipe.Id}", recipe);
    }
}