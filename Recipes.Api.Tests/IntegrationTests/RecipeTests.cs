using System.Net;
using System.Net.Http.Json;
using Recipes.Api.Dto;
using Recipes.Api.Data;
using Recipes.Api.Entities;
using Microsoft.Extensions.DependencyInjection;
namespace Recipes.Api.Tests;


public class RecipeTests
{
    [Fact]
    public async Task CreateRecipe_ReturnsCreated()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();


        var request = new CreateRecipeDto
        {
            Name= "Test recipe",
            Description = "Test",
            RecipeIngredients = new List<CreateRecipeIngredientDto>
            {
                new CreateRecipeIngredientDto
                {
                    Name = "Sugar",
                    Gram = 100
                }
            }
        };

        var response = await client.PostAsJsonAsync("/recipes", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipes_EmptyRecipe_HasZeroKcal()
    {
        using var factory = new CustomWebApplicationFactory();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Recipes.Add(new Recipe
            {
                Id = Guid.NewGuid(),
                Name= "Empty",
                Description = "No ingredients"
            });


            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var recipes = await client.GetFromJsonAsync<PagedResponseDto<RecipeDto>>("/recipes");

        Assert.NotNull(recipes);
        Assert.Single(recipes.Items);
        Assert.Equal(0, recipes.Items[0].Kcal);
    }
    [Fact]
    public async Task GetRecipes_IngredientNoKcal_HasZeroKcal()
    {
        using var factory = new CustomWebApplicationFactory();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                Name= "Empty",
                Description = "No ingredients"
            };
            var ingredient = new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "NoKcal"
            };
            db.Recipes.Add(recipe);
            db.Ingredients.Add(ingredient);
            db.RecipeIngredients.Add(new RecipeIngredient
            {
                RecipeId = recipe.Id,
                IngredientId = ingredient.Id,
                Gram = 100
            });
            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var recipes = await client.GetFromJsonAsync<PagedResponseDto<RecipeDto>>("/recipes");

        Assert.NotNull(recipes);
        Assert.Single(recipes.Items);
        Assert.Equal(0, recipes.Items[0].Kcal);
    }
    [Fact]
    public async Task GetRecipes_IngredientHasKcal_HasSumKcal()
    {
        using var factory = new CustomWebApplicationFactory();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                Name= "recipe",
                Description = ""
            };
            var ingredient = new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Kcal100g = 100,
            };
            db.Recipes.Add(recipe);
            db.Ingredients.Add(ingredient);
            db.RecipeIngredients.Add(new RecipeIngredient
            {
                RecipeId = recipe.Id,
                IngredientId = ingredient.Id,
                Gram = 100
            });
            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var recipes = await client.GetFromJsonAsync<PagedResponseDto<RecipeDto>>("/recipes");

        Assert.NotNull(recipes);
        Assert.Single(recipes.Items);
        Assert.Equal(100, recipes.Items[0].Kcal);
    }
}