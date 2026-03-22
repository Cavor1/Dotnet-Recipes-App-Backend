using System.Net;
using System.Net.Http.Json;
using Recipes.Api.Dto;
namespace Recipes.Api.Tests;

public class RecipeTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RecipeTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateRecipe_ReturnsCreated()
    {
        var request = new RecipeDto
        {
            Title = "Test recipe",
            Description = "Test",
            Ingredients = new List<RecipeIngredientDto>
            {
                new RecipeIngredientDto
                {
                    Name = "Sugar",
                    Quantity = "100g"
                }

            }
        };

        var response = await _client.PostAsJsonAsync("/recipes", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}