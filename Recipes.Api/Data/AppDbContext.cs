using Microsoft.EntityFrameworkCore;
namespace Recipes.Api.Data;

using Recipes.Api.Entities;

public class AppDbContext : DbContext// DbContext - main class from ef
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }//call default constructor with options
    public DbSet<Recipe> Recipes => Set<Recipe>(); //Set<T>() inherited from DbContext
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<MealIngredient> MealIngredients => Set<MealIngredient>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecipeIngredient>().HasKey(ri => new { ri.RecipeId, ri.IngredientId });//composite key
        modelBuilder.Entity<Ingredient>().HasIndex(i => i.Name).IsUnique(); //unique name and index on it
        modelBuilder.Entity<MealIngredient>().HasKey(ri => new { ri.MealId, ri.IngredientId });
    }
}

