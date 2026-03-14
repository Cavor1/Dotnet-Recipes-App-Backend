namespace Recipes.Api.Dto;
using System.ComponentModel.DataAnnotations;

public class RecipeDto
{
    public Guid Id {get;set;}
    public string Title {get;set;} = "";
    public string? Description {get;set;}
    public List<RecipeIngredientDto> Ingredients {get;set;} = new();
}

public class CreateRecipeDto
{
    [Required]
    [MinLength(1)]
    public string Title {get;set;} = "";
    public string? Description {get;set;}
    [Required]
    public List<CreateRecipeIngredientDto> RecipeIngredients {get;set;} = new();
}
public class RecipeIngredientDto
{

    public Guid IngredientID {get;set;}
    [Required]
    [MinLength(1)]
    public string Name {get;set;} = "";
    public string? Quantity {get;set;}
}

public class CreateRecipeIngredientDto
{
    [Required]
    [MinLength(1)]
    public string Name {get;set;} = "";
    public string? Quantity {get;set;}
}