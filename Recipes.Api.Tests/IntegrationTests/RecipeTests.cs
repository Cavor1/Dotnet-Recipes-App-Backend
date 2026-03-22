using System.Net;
using System.Net.Http.Json;
using Recipes.Api.Dto;
namespace Recipes.Api.Tests;

// public class RecipeTests : IClassFixture<CustomWebApplicationFactory>
// {
//     private readonly HttpClient _client;

//     public RecipeTests(CustomWebApplicationFactory factory)
//     {
//         _client = factory.CreateClient();
//     }

//     [Fact]
//     public async Task CreateRecipe_ReturnsCreated()
//     {
//         var request = new CreateRecipeDto
//         {
//             Title = "Test recipe",
//             Description = "Test",
//             RecipeIngredients = new List<CreateRecipeIngredientDto>
//             {
//                 new CreateRecipeIngredientDto
//                 {
//                     Name = "Sugar",
//                     Quantity = "100g"
//                 }

//             }
//         };

//         var response = await _client.PostAsJsonAsync("/recipes", request);

//         Assert.Equal(HttpStatusCode.Created, response.StatusCode);
//     }
// }
public class CreateRecipeTests
{
[Fact]
public async Task CreateRecipe_ReturnsCreated()
{
    using var factory = new CustomWebApplicationFactory();
    using var client = factory.CreateClient();

    var request = new CreateRecipeDto
    {
        Title = "Test recipe",
        Description = "Test",
        RecipeIngredients = new List<CreateRecipeIngredientDto>
        {
            new CreateRecipeIngredientDto
            {
                Name = "Sugar",
                Quantity = "100g"
            }
        }
    };

    var response = await client.PostAsJsonAsync("/recipes", request);

    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
}
}