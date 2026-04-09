
using System.Net;
using System.Net.Http.Json;
using Recipes.Api.Dto;
using Recipes.Api.Data;
using Recipes.Api.Entities;
using Microsoft.Extensions.DependencyInjection;
namespace Recipes.Api.Tests;


public class EatMealTests
{
    
    [Fact]
    public async Task EatMeal_EatAndUndo_MealIsEatenThenNot()
    {
        using var factory = new CustomWebApplicationFactory();

        var mealId = Guid.NewGuid();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Meals.Add(new Meal
            {
                Id = mealId,
                Name = "name",
            });
            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient();
        await client.PatchAsync($"/meals/{mealId}/eat", null);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var meal = await db.Meals.FindAsync(mealId);

            Assert.NotNull(meal);
            Assert.NotNull(meal!.EatenTime);
        }

        await client.PatchAsync($"/meals/{mealId}/undoeat", null);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var meal = await db.Meals.FindAsync(mealId);

            Assert.NotNull(meal);
            Assert.Null(meal!.EatenTime);
        }
    }
}