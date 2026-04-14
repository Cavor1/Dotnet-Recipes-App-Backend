namespace Recipes.Api.Dto;
using System.ComponentModel.DataAnnotations;

public class RecipeDto
{
    public Guid Id {get;set;}
    public string Name{get;set;} = "";
    public string? Description {get;set;}
    public double? Kcal {get;set;}
    public List<RecipeIngredientDto> Ingredients {get;set;} = new();
}

public class CreateRecipeDto
{
    [Required]
    [MinLength(1)]
    public string Name{get;set;} = "";
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
    public double? Gram {get;set;}
}

public class CreateRecipeIngredientDto
{
    [Required]
    [MinLength(1)]
    public string Name {get;set;} = "";
    public double? Gram {get;set;}
    public double? Kcal100g {get;set;}

}

public class GetRecipesQueryDto
{
    public string? SearchString {get;set;}

    [Range(1,int.MaxValue)]
    public int? Page {get;set;}

    [Range(1,200)]
    public int? PageSize {get;set;}
}