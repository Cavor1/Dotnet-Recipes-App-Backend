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
            Kcal = r.MealIngredients.Sum(ri => ri.Gram*ri.Ingredient.Kcal100g/100) ?? 0,
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
    static async Task<IResult> CreateMeal(CreateMealDto req, AppDbContext db)
    {
        
        //validation
        var reqValidation = req.Validate();
        if (reqValidation is not null) return reqValidation;

        foreach (var ri in req.MealIngredients)
        {
            var riValidation = ri.Validate();
            if (riValidation is not null) return riValidation;
        }

        var reqIngredients = req.MealIngredients.Select(r => new
        {
            Name = r.Name.Trim().ToLowerInvariant(),
            Gram = r.Gram,
            Kcal100g = r.Kcal100g
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

        var meal = new Meal()
        {
            Id = Guid.NewGuid(),
            Name= req.Name,
            RecipeId = req.RecipeId
        };

        var existingIngredients = await db.Ingredients
            .Where(ei => reqIngredients
                .Select(ri => ri.Name)
                .Contains(ei.Name)) //list of reqIngredients.Name contains ei.Name
            .ToDictionaryAsync(i => i.Name);

        foreach (var reqIngredient in reqIngredients)
        {

            var mealIngredient = new MealIngredient()
            {
                MealId = meal.Id,
                Gram = reqIngredient.Gram
            };

            if (!existingIngredients.ContainsKey(reqIngredient.Name))
            {
                var newIngredient = new Ingredient()
                {
                    Id = Guid.NewGuid(),
                    Name = reqIngredient.Name,
                    Kcal100g = reqIngredient.Kcal100g
                };
                db.Ingredients.Add(newIngredient);
                mealIngredient.IngredientId = newIngredient.Id;
            }
            else
            {
                mealIngredient.IngredientId = existingIngredients[reqIngredient.Name].Id;
            }
            db.MealIngredients.Add(mealIngredient);

        }

        db.Meals.Add(meal);
        await db.SaveChangesAsync();

        return Results.Created($"/recipes/{meal.Id}", new {Id = meal.Id});//[TODO] can retturn better response
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

