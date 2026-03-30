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
    public static void MapRecipesEndpoints(this WebApplication app)
    {
        app.MapGet("/recipes", GetMeals);
        app.MapGet("/recipes/{id:guid}", GetMeal);
        app.MapPost("/recipes", CreateMeal);
        app.MapPut("/recipes/{id:guid}", UpdateMeal);
        app.MapDelete("/recipes/{id:guid}", DeleteMeal);
    }
    static async Task<IResult> GetMeals(AppDbContext db)
    {
        return Results.StatusCode(501);
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

