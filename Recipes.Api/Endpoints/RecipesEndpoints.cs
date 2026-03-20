using System.Data.Common;
using System.Net;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http.HttpResults;
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
        app.MapPut("/recipes/{id:guid}", UpdateRecipe);
        app.MapDelete("/recipes/{id:guid}", DeleteRecipe);
    }
    //[TODO] is it problem to return descriptions of all recipes?
    static async Task<IResult> GetRecipes(AppDbContext db)
    {
        var recipes = await db.Recipes.Select(r => new RecipeDto()
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description, 
            Ingredients = r.RecipeIngredients.Select(ri => new RecipeIngredientDto
            {
                IngredientID = ri.IngredientId,
                Name = ri.Ingredient.Name,
                Quantity = ri.Quantity
            }).ToList()
        }).ToListAsync();
        return Results.Ok(recipes);

    }
    static async Task<IResult> GetRecipe(Guid id,AppDbContext db)
    {
        var recipe = await db.Recipes.Where(r => r.Id == id).Select(r => new RecipeDto()
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Ingredients = r.RecipeIngredients.Select(ri => new RecipeIngredientDto
                {
                    IngredientID = ri.IngredientId,
                    Name = ri.Ingredient.Name,
                    Quantity = ri.Quantity
                }).ToList()
            }).FirstOrDefaultAsync();
        return recipe is null
            ? Results.NotFound()
            : Results.Ok(recipe);
    }

    //[TODO]might have concurrency problems, between getting existing ingredients and saving new ingredients some time passes.
    static async Task<IResult> CreateRecipe(CreateRecipeDto req, AppDbContext db)
    {
        
        //validation
        var reqValidation = req.Validate();
        if (reqValidation is not null) return reqValidation;

        foreach (var ri in req.RecipeIngredients)
        {
            var riValidation = ri.Validate();
            if (riValidation is not null) return riValidation;
        }

        var reqIngredients = req.RecipeIngredients.Select(r => new
        {
            Name = r.Name.Trim().ToLowerInvariant(),
            Quantity = r.Quantity
        }).ToList();

        //check for duplicates, is there are, bad request
        var duplicateNames = reqIngredients
            .GroupBy(x => x.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateNames.Count > 0)
        {
            return Results.BadRequest(new
            {
                Error = "Duplicate ingredient names in request",
                Ingredients = duplicateNames
            });
        }

        //get existing ingredients
        //create and add nonexistent
        //create recipeingredients and add to database

        var recipe = new Recipe()
        {
            Id = Guid.NewGuid(),
            Title = req.Title,
            Description = req.Description
        };

        var existingIngredients = await db.Ingredients
            .Where(ei => reqIngredients
                .Select(ri => ri.Name)
                .Contains(ei.Name)) //list of reqIngredients.Name contains ei.Name
            .ToDictionaryAsync(i => i.Name);

        foreach (var reqIngredient in reqIngredients)
        {

            var recipeIngredient = new RecipeIngredient()
            {
                RecipeId = recipe.Id,
                Quantity = reqIngredient.Quantity
            };

            if (!existingIngredients.ContainsKey(reqIngredient.Name))
            {
                var newIngredient = new Ingredient()
                {
                    Id = Guid.NewGuid(),
                    Name = reqIngredient.Name
                };
                db.Ingredients.Add(newIngredient);
                recipeIngredient.IngredientId = newIngredient.Id;
            }
            else
            {
                recipeIngredient.IngredientId = existingIngredients[reqIngredient.Name].Id;
            }
            db.RecipeIngredients.Add(recipeIngredient);

        }

        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();

        return Results.Created($"/recipes/{recipe.Id}", new {Id = recipe.Id});//[TODO] can retturn better response
    }

    static async Task<IResult> UpdateRecipe(Guid id, CreateRecipeDto req,AppDbContext db)
    {
        //validation
        var reqValidation = req.Validate();
        if (reqValidation is not null) return reqValidation;

        foreach (var ri in req.RecipeIngredients)
        {
            var riValidation = ri.Validate();
            if (riValidation is not null) return riValidation;
        }

        //getting recipe and return if not found by id
        var recipe = await db.Recipes.Where(r => r.Id == id).FirstOrDefaultAsync();

        if (recipe is null) return Results.NotFound();

        var reqIngredients = req.RecipeIngredients.Select(r => new
        {
            Name = r.Name.Trim().ToLowerInvariant(),
            Quantity = r.Quantity
        }).ToList();

        //check for duplicates, is there are, bad request
        var duplicateNames = reqIngredients
            .GroupBy(x => x.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateNames.Count > 0)
        {
            return Results.BadRequest(new
            {
                Error = "Duplicate ingredient names in request",
                Ingredients = duplicateNames
            });
        }

        //get existing ingredients
        var existingIngredients = await db.Ingredients
            .Where(ei => reqIngredients
                .Select(ri => ri.Name)
                .Contains(ei.Name)) //list of reqIngredients.Name contains ei.Name
            .ToDictionaryAsync(i => i.Name);      

        //set new name and description of recipe
        recipe.Title = req.Title;
        recipe.Description = req.Description;


        //delete recipeingredients
        //[TODO] ?delete only duplicates
        await db.RecipeIngredients.Where(ri => ri.RecipeId == id).ExecuteDeleteAsync();

        //add recipe ingredients and new ingredients
        foreach (var reqIngredient in reqIngredients)
        {

            var recipeIngredient = new RecipeIngredient()
            {
                RecipeId = recipe.Id,
                Quantity = reqIngredient.Quantity
            };

            if (!existingIngredients.ContainsKey(reqIngredient.Name))
            {
                var newIngredient = new Ingredient()
                {
                    Id = Guid.NewGuid(),
                    Name = reqIngredient.Name
                };
                db.Ingredients.Add(newIngredient);
                recipeIngredient.IngredientId = newIngredient.Id;
            }
            else
            {
                recipeIngredient.IngredientId = existingIngredients[reqIngredient.Name].Id;
            }
            db.RecipeIngredients.Add(recipeIngredient);

        }
        await db.SaveChangesAsync();


        return Results.Ok();
    }



    static async Task<IResult> DeleteRecipe(Guid id,AppDbContext db)
    {
        var recipe = await db.Recipes.Where(r => r.Id == id).FirstOrDefaultAsync();
        
        if (recipe is null) return Results.NotFound();

        db.Recipes.Remove(recipe);
        await db.SaveChangesAsync();

        return Results.Ok();   
    }
}