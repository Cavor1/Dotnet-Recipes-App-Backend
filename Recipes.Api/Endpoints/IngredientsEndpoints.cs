using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Recipes.Api.Data;
using Recipes.Api.Dto;
using Recipes.Api.Entities;
using Recipes.Api.Validation;

namespace Recipes.Api.Endpoints;

public static class IngredientsEndpoints
{
    public static void MapIngredientsEndpoints(this WebApplication app)
    {
        app.MapGet("/ingredients", GetIngredients);
        app.MapGet("/ingredients/{id:guid}", GetIngredient);
        app.MapPost("/ingredients", CreateIngredient);
    //     app.MapPut("/recipes/{id:guid}", UpdateRecipe);
    //     app.MapDelete("/recipes/{id:guid}", DeleteRecipe);
    }
    static async Task<IResult> GetIngredients(AppDbContext db)
    {
        var ingredients = await db.Ingredients.Select(r => new IngredientDto()
        {
            Id = r.Id,
            Name = r.Name,
        }).ToListAsync();
        return Results.Ok(ingredients);
    }
    static async Task<IResult> GetIngredient(Guid id,AppDbContext db)
    {
        return Results.Ok();
    }

    static async Task<IResult> CreateIngredient(CreateIngredientDto req, AppDbContext db)
    {

        return Results.Ok();
    }
}