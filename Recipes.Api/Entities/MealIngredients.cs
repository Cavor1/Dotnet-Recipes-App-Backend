namespace Recipes.Api.Entities;

public class MealIngredient
{
    public Guid MealId { get; set; }
    public Meal Meal { get; set; } = null!;

    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public double? Gram { get; set; }
}