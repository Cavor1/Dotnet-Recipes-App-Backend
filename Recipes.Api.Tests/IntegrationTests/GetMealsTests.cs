using System.Net;
using System.Net.Http.Json;
using Recipes.Api.Dto;
using Recipes.Api.Data;
using Recipes.Api.Entities;
using Microsoft.Extensions.DependencyInjection;
namespace Recipes.Api.Tests;

public class GetMealsTests
{
    [Fact]
    public async Task GetMeals_WithFrom_ReturnsMealsFromThatDate()
    {
        using var factory = new CustomWebApplicationFactory();

        var olderMeal = new Meal
        {
            Id = Guid.NewGuid(),
            Name = "Older",
            EatenTime = new DateTime(2026, 4, 1, 10, 0, 0)
        };

        var newerMeal = new Meal
        {
            Id = Guid.NewGuid(),
            Name = "Newer",
            EatenTime = new DateTime(2026, 4, 5, 10, 0, 0)
        };

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Meals.AddRange(olderMeal, newerMeal);
            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var meals = await client.GetFromJsonAsync<List<MealDto>>("/meals?from=2026-04-03");

        Assert.NotNull(meals);
        Assert.Single(meals);
        Assert.Equal(newerMeal.Id, meals[0].Id);
    }

    [Fact]
    public async Task GetMeals_WithTo_ReturnsMealsUpToThatDate()
    {
        using var factory = new CustomWebApplicationFactory();

        var earlierMeal = new Meal
        {
            Id = Guid.NewGuid(),
            Name = "Earlier",
            EatenTime = new DateTime(2026, 4, 2, 10, 0, 0)
        };

        var laterMeal = new Meal
        {
            Id = Guid.NewGuid(),
            Name = "Later",
            EatenTime = new DateTime(2026, 4, 8, 10, 0, 0)
        };

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Meals.AddRange(earlierMeal, laterMeal);
            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var meals = await client.GetFromJsonAsync<List<MealDto>>("/meals?to=2026-04-03");

        Assert.NotNull(meals);
        Assert.Single(meals);
        Assert.Equal(earlierMeal.Id, meals[0].Id);
    }

    [Fact]
    public async Task GetMeals_WithEatenTrue_ReturnsOnlyEatenMeals()
    {
        using var factory = new CustomWebApplicationFactory();

        var eatenMeal = new Meal
        {
            Id = Guid.NewGuid(),
            Name = "Eaten",
            EatenTime = new DateTime(2026, 4, 4, 12, 0, 0)
        };

        var notEatenMeal = new Meal
        {
            Id = Guid.NewGuid(),
            Name = "Not eaten",
            EatenTime = null
        };

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Meals.AddRange(eatenMeal, notEatenMeal);
            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var meals = await client.GetFromJsonAsync<List<MealDto>>("/meals?eaten=true");

        Assert.NotNull(meals);
        Assert.Single(meals);
        Assert.Equal(eatenMeal.Id, meals[0].Id);
    }

    [Fact]
    public async Task GetMeals_WithEatenFalse_ReturnsOnlyNotEatenMeals()
    {
        using var factory = new CustomWebApplicationFactory();

        var eatenMeal = new Meal
        {
            Id = Guid.NewGuid(),
            Name = "Eaten",
            EatenTime = new DateTime(2026, 4, 4, 12, 0, 0)
        };

        var notEatenMeal = new Meal
        {
            Id = Guid.NewGuid(),
            Name = "Not eaten",
            EatenTime = null
        };

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Meals.AddRange(eatenMeal, notEatenMeal);
            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var meals = await client.GetFromJsonAsync<List<MealDto>>("/meals?eaten=false");

        Assert.NotNull(meals);
        Assert.Single(meals);
        Assert.Equal(notEatenMeal.Id, meals[0].Id);
    }

    [Fact]
    public async Task GetMeals_WithFromAndTo_ReturnsMealsInRange()
    {
        using var factory = new CustomWebApplicationFactory();

        var beforeRange = new Meal
        {
            Id = Guid.NewGuid(),
            Name = "Before range",
            EatenTime = new DateTime(2026, 4, 1, 10, 0, 0)
        };

        var inRange = new Meal
        {
            Id = Guid.NewGuid(),
            Name = "In range",
            EatenTime = new DateTime(2026, 4, 5, 10, 0, 0)
        };

        var afterRange = new Meal
        {
            Id = Guid.NewGuid(),
            Name = "After range",
            EatenTime = new DateTime(2026, 4, 10, 10, 0, 0)
        };

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Meals.AddRange(beforeRange, inRange, afterRange);
            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var meals = await client.GetFromJsonAsync<List<MealDto>>("/meals?from=2026-04-03&to=2026-04-07");

        Assert.NotNull(meals);
        Assert.Single(meals);
        Assert.Equal(inRange.Id, meals[0].Id);
    }

    [Fact]
    public async Task GetMeals_WithInvalidFrom_ReturnsBadRequest()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/meals?from=null");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}