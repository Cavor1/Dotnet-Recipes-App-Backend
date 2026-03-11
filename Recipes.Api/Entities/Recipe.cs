namespace Recipes.Api.Entities;

public class Recipe
{
    public Guid Id {get;set;}//sets automatically ID as primary key
    public string Title {get;set;} = ""; //not NULL
    public string? Description {get;set;}

    public List<RecipeIngredient> RecipeIngredients {get;set;} = new();

}