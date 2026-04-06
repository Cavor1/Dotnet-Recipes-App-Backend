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

public static class MealEndpoints
{
    public static void MapMealsEndpoints(this WebApplication app)
    {
        app.MapGet("/meals", GetMeals);
        app.MapGet("/meals/{id:guid}", GetMeal);
        app.MapPost("/meals", CreateMeal);
        app.MapPut("/meals/{id:guid}", UpdateMeal);
        app.MapDelete("/meals/{id:guid}", DeleteMeal);
    }
    static async Task<IResult> GetMeals(AppDbContext db) //query params
    {
        var meals = await db.Meals.Select(r => new MealDto()
        {
            Id = r.Id,
            Name= r.Name,
            RecipeId = r.RecipeId,
            EatenTime = r.EatenTime,
            Kcal = r.Kcal ?? r.MealIngredients.Sum(ri => ri.Gram*ri.Ingredient.Kcal100g/100) ?? 0,
            Ingredients = r.MealIngredients.Select(ri => new MealIngredientDto
            {
                IngredientID = ri.IngredientId,
                Name = ri.Ingredient.Name,
                Gram = ri.Gram
            }).ToList()
        }).ToListAsync();
        return Results.Ok(meals);

    }
    static async Task<IResult> GetMeal(Guid id,AppDbContext db)
    {
        return Results.StatusCode(501);
    }
    static async Task<IResult> CreateMeal(AppDbContext db)
    {
        return Results.StatusCode(501);
    }
    static async Task<IResult> DeleteMeal(Guid id,AppDbContext db)
    {
        return Results.StatusCode(501);
    }
    static async Task<IResult> UpdateMeal(Guid id,AppDbContext db)
    {
        return Results.StatusCode(501);
    }

}

