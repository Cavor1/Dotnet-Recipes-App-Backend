
using System.Net;
using System.Net.Http.Json;
using Recipes.Api.Dto;
using Recipes.Api.Data;
using Recipes.Api.Entities;
using Microsoft.Extensions.DependencyInjection;
namespace Recipes.Api.Tests;


public class MealTests
{
[Fact]
    public async Task CreateMeal_WithIngredients_ReturnsCreated()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateMealDto
        {
            Name = "Breakfast",
            MealIngredients = new List<CreateMealIngredientDto>
            {
                new CreateMealIngredientDto
                {
                    Name = "Bread",
                    Gram = 100,
                    Kcal100g = 250
                },
                new CreateMealIngredientDto
                {
                    Name = "Butter",
                    Gram = 20,
                    Kcal100g = 720
                }
            }
        };

        var response = await client.PostAsJsonAsync("/meals", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateMeal_WithEmptyName_ReturnsBadRequest()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateMealDto
        {
            Name = "",
            MealIngredients = new List<CreateMealIngredientDto>
            {
                new CreateMealIngredientDto
                {
                    Name = "Bread",
                    Gram = 100,
                    Kcal100g = 250
                }
            }
        };

        var response = await client.PostAsJsonAsync("/meals", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMeal_WithEmptyIngredientsList_ReturnsCreated()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateMealDto
        {
            Name = "Breakfast",
            MealIngredients = new List<CreateMealIngredientDto>()
        };

        var response = await client.PostAsJsonAsync("/meals", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateMeal_WithIngredientEmptyName_ReturnsBadRequest()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateMealDto
        {
            Name = "Breakfast",
            MealIngredients = new List<CreateMealIngredientDto>
            {
                new CreateMealIngredientDto
                {
                    Name = "",
                    Gram = 100,
                    Kcal100g = 250
                }
            }
        };

        var response = await client.PostAsJsonAsync("/meals", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMeal_SavesMealInDatabase()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new CreateMealDto
        {
            Name = "Breakfast",
            MealIngredients = new List<CreateMealIngredientDto>
            {
                new CreateMealIngredientDto
                {
                    Name = "Bread",
                    Gram = 100,
                    Kcal100g = 250
                }
            }
        };

        var response = await client.PostAsJsonAsync("/meals", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var meal = db.Meals.Single();
        Assert.Equal("Breakfast", meal.Name);

        var mealIngredient = db.MealIngredients.Single();
        Assert.Equal(100, mealIngredient.Gram);
    }

    [Fact]
    public async Task CreateMeal_WithRecipeId_ReturnsCreated()
    {
        using var factory = new CustomWebApplicationFactory();
        Guid recipeId;

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var ingredient = new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "rice",
                Kcal100g = 130
            };

            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                Name = "Rice recipe",
                Description = "Simple rice",
                RecipeIngredients = new List<RecipeIngredient>()
            };

            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                RecipeId = recipe.Id,
                IngredientId = ingredient.Id,
                Gram = 200
            });

            db.Ingredients.Add(ingredient);
            db.Recipes.Add(recipe);

            await db.SaveChangesAsync();
            recipeId = recipe.Id;
        }

        using var client = factory.CreateClient();

        var request = new CreateMealDto
        {
            Name = "Lunch",
            RecipeId = recipeId,
            MealIngredients = new List<CreateMealIngredientDto>
            {
                new CreateMealIngredientDto
                {
                    Name = "rice",
                    Gram = 200,
                    Kcal100g = 130
                }
            }
        };

        var response = await client.PostAsJsonAsync("/meals", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }    
}