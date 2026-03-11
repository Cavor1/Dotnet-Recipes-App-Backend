namespace Recipes.Api.Dto;
using System.ComponentModel.DataAnnotations;

public class RecipeIngredientDto
{
    public Guid Id {get;set;}
    public string Title {get;set;} = "";
    public string? Description {get;set;}
    public List<List<string>>? Ingredients {get;set;} 
}
public class CreateRecipeIngredientDto
{
    [Required]
    public string Title {get;set;} = "";
    public string? Description {get;set;}
    public List<List<string>>? Ingredients {get;set;} 
}