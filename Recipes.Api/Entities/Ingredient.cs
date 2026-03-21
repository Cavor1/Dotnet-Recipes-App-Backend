namespace Recipes.Api.Entities;

public class Ingredient
{
    public Guid Id {get;set;}//sets automatically ID as primary key
    public string Name {get;set;} = ""; //not NULL
    public double? Kcal100g {get;set;} //Kcal/100g
    public List<RecipeIngredient> RecipeIngredients {get;set;} = new();

}