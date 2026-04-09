
using System.Net;
using System.Net.Http.Json;
using Recipes.Api.Dto;
using Recipes.Api.Data;
using Recipes.Api.Entities;
using Microsoft.Extensions.DependencyInjection;
namespace Recipes.Api.Tests;


public class GetMealTests
{
    
    [Fact]
    public async Task GetMeal_ExistingId_ReturnsOk()
    {
        using var factory = new CustomWebApplicationFactory();

        var mealDb = new Meal
        {
            Id = Guid.NewGuid(),
            Name= "name",
        };

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Meals.Add(mealDb);
            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient();
        var meal = await client.GetFromJsonAsync<MealDto>($"/meals/{mealDb.Id}");

        Assert.NotNull(meal);
    }
}