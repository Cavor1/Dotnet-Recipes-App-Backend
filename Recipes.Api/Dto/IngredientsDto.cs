namespace Recipes.Api.Dto;
using System.ComponentModel.DataAnnotations;

public class IngredientDto
{
    public Guid Id {get;set;}
    [Required]
    [MinLength(1)]
    public string Name {get;set;} = "";

    public double? Kcal100g {get;set;}
}

public class CreateIngredientDto
{
    public Guid Id {get;set;}
    [Required]
    [MinLength(1)]
    public string Name {get;set;} = "";
    public double? Kcal100g {get;set;}
}

public class GetIngredientsQueryDto
{
    public string? SearchString {get;set;}
    [Range(1,int.MaxValue)]
    public int? Page {get;set;}
    [Range(1,200)]
    public int? PageSize {get;set;}
}