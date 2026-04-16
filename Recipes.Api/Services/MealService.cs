using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Recipes.Api.Data;
using Recipes.Api.Dto;
using Recipes.Api.Entities;

namespace Recipes.Api.Services;

public class MealService
{

    public static async Task<Result<Meal>> UpdateMealService(Meal meal, CreateMealDto req, AppDbContext db)
    {
        
        meal.Name = req.Name;
        meal.RecipeId = req.RecipeId;
        await db.MealIngredients.Where(mi => mi.MealId == meal.Id).ExecuteDeleteAsync();

        var result = await FillMealIngredients(meal,req,db);
        return result;
    }
    public static async Task<Result<Meal>> CreateMealService(CreateMealDto req, AppDbContext db)
    {

        var meal = new Meal()
        {
            Id = Guid.NewGuid(),
            Name= req.Name,
            RecipeId = req.RecipeId
        };
        db.Meals.Add(meal);
        await db.SaveChangesAsync();
        var result = await FillMealIngredients(meal,req,db);
        return result;
    }
    private static async Task<Result<Meal>> FillMealIngredients(Meal meal, CreateMealDto req, AppDbContext db)
    {

        var reqIngredients = req.MealIngredients.Select(r => new
        {
            Name = r.Name.Trim().ToLowerInvariant(),
            Gram = r.Gram,
            Kcal100g = r.Kcal100g
        }).ToList();

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

        await db.SaveChangesAsync();
        return new Result<Meal> {Value = meal};
    }


}
