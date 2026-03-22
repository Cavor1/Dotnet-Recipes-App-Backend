using System.Net;
using System.Net.Http.Json;
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
        var request = new
        {
            Title = "Test recipe",
            Description = "Test",
            RecipeIngredients = new[]
            {
                new { Name = "Sugar", Quantity = "100g" }
            }
        };

        var response = await _client.PostAsJsonAsync("/recipes", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}