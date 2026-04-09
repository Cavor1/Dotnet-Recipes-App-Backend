using System.Net;
using System.Net.Http.Json;
using Recipes.Api.Dto;
using Recipes.Api.Data;
using Recipes.Api.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
namespace Recipes.Api.Tests;


public class UpdateMealTests
{
    [Fact]
    public async Task UpdateMeal_ExistingMealWithIngredients_MealUpdated()
    {
        using var factory = new CustomWebApplicationFactory();

        var mealId = Guid.NewGuid();
        var ingredientID = Guid.NewGuid();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Meals.Add(new Meal
            {
                Id = mealId,
                Name = "name",
            });
            db.Ingredients.Add(new Ingredient
            {
                Id = ingredientID,
                Name = "i" 
            });
            db.MealIngredients.Add(new MealIngredient
            {
                MealId = mealId,
                IngredientId = ingredientID,
                Gram = 50
            });
            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient();
        await client.PutAsJsonAsync($"/meals/{mealId}", new CreateMealDto
        {
            Name = "name2",
            MealIngredients = new List<CreateMealIngredientDto>()
            {
                new CreateMealIngredientDto
                {
                    Name = "i",
                    Gram = 100
                }
            }

        });

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var meal = await db.Meals
                .Include(m => m.MealIngredients)
                .SingleAsync(m => m.Id == mealId);

            Console.WriteLine($"Meal: {meal.Name} ({meal.Id})");

            foreach (var mi in meal.MealIngredients)
            {
                Console.WriteLine($" - Ingredient: {mi.Ingredient?.Name}, Gram: {mi.Gram}, IngredientId: {mi.IngredientId}");
            }

            Assert.NotNull(meal);
            Assert.Equal("name2",meal.Name);
            Assert.Equal(100,meal.MealIngredients.Single(mi => mi.IngredientId == ingredientID).Gram);
        }
    }
}