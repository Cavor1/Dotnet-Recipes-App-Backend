namespace Recipes.Api.Entities;

public class Meal
{
    public Guid Id {get;set;}//sets automatically ID as primary key
    public string Name{get;set;} = ""; //not NULL
    public DateTime? EatenTime {get;set;}
    //public DateTime? CreatedTime {get;set;}
    public double Kcal {get;set;} //to increase speed of retrieval, 
    public List<MealIngredient> MealIngredients {get;set;} = new();

}

//stores actual ingredients used, or kcal
//recipe used or null
