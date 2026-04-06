namespace Recipes.Api.Dto;
using System.ComponentModel.DataAnnotations;

public class MealDto
{
    public Guid Id {get;set;}
    public Guid? RecipeId {get;set;}
    public double? Kcal {get;set;}
    public string Name{get;set;} = "";
    public DateTime? EatenTime {get;set;} 

    public List<MealIngredientDto> Ingredients {get;set;} = new();

}

public class CreateMealDto
{
    [Required]
    [MinLength(1)]
    public string Name{get;set;} = "";
    public string? Description {get;set;}
    [Required]
    public List<CreateMealIngredientDto> MealIngredients {get;set;} = new();
}
public class MealIngredientDto
{

    public Guid IngredientID {get;set;}
    [Required]
    [MinLength(1)]
    public string Name {get;set;} = "";
    public double? Gram {get;set;}
    public double? Kcal100g {get;set;}

}

public class CreateMealIngredientDto
{
    [Required]
    [MinLength(1)]
    public string Name {get;set;} = "";
    public double? Gram {get;set;}
}