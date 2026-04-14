using Microsoft.EntityFrameworkCore;
using Recipes.Api.Data;
using Recipes.Api.Dto;
using Recipes.Api.Entities;
using Recipes.Api.Validation;

namespace Recipes.Api.Endpoints;

public static class IngredientsEndpoints
{
    public static void MapIngredientsEndpoints(this WebApplication app)
    {
        app.MapGet("/ingredients", GetIngredients);
        app.MapGet("/ingredients/{id:guid}", GetIngredient);
        app.MapPost("/ingredients", CreateIngredient);
        app.MapPut("/ingredients/{id:guid}", UpdateIngredient);
        app.MapDelete("/ingredients/{id:guid}", DeleteIngredient);
    }
    static async Task<IResult> GetIngredients([AsParameters] GetIngredientsQueryDto req, AppDbContext db)
    {

        var page = req.Page ?? 1;
        var pageSize = req.PageSize ?? 20;
        
        var reqValidation = req.Validate();
        if (reqValidation is not null) return reqValidation;

        var totalCount = await db.Ingredients.CountAsync();

        var query = db.Ingredients.AsQueryable();
        if (req.SearchString != null)
        {
           query = query.Where(i => i.Name.Contains(req.SearchString.ToLowerInvariant()));
        }
        var ingredients = await query.Skip((page-1)*pageSize).Take(pageSize).Select(r => new IngredientDto()
        {
            Id = r.Id,
            Name = r.Name,
            Kcal100g = r.Kcal100g
        }).ToListAsync();
        return Results.Ok(new PagedResponseDto<IngredientDto>
        {
            Items = ingredients,
            Metadata = new PageMetadataDto
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount/pageSize)
            }   
        });
    }
    static async Task<IResult> GetIngredient(Guid id,AppDbContext db)
    {
        var ingredient = await db.Ingredients.Where(i => i.Id == id).Select(i => new IngredientDto
        {
            Id = i.Id,
            Name = i.Name,
            Kcal100g = i.Kcal100g

        }).FirstOrDefaultAsync();

        
        return ingredient is null
            ? Results.NotFound()
            : Results.Ok(ingredient);
    }

    static async Task<IResult> CreateIngredient(CreateIngredientDto req, AppDbContext db)
    {

        var reqValidation = req.Validate();
        if (reqValidation is not null) return reqValidation;


        var ingredient = new Ingredient
        {
          Id = Guid.NewGuid(),
          Name = req.Name.Trim().ToLowerInvariant(),
          Kcal100g = req.Kcal100g

        };
        db.Ingredients.Add(ingredient);

        //error if unique constrait on name violated
        try
        {
            await db.SaveChangesAsync();

        }
        catch(DbUpdateException)
        {
            return Results.BadRequest();
        }
            
        return Results.Created($"/ingredients/{ingredient.Id}", new {Id = ingredient.Id});
    }
    static async Task<IResult> UpdateIngredient(Guid id, CreateIngredientDto req, AppDbContext db)
    {
        var reqValidation = req.Validate();
        if (reqValidation is not null) return reqValidation;

        var ingredient = await db.Ingredients.Where(i => i.Id == id).FirstOrDefaultAsync();
        if (ingredient is null) return Results.NotFound();

        ingredient.Name = req.Name.Trim().ToLowerInvariant();
        ingredient.Kcal100g = req.Kcal100g;

        try
        {
            await db.SaveChangesAsync();
            return Results.Ok();
        }
        catch (DbUpdateException)
        {
            return Results.Conflict("ingredient name already exist");   
        }
    }
    static async Task<IResult> DeleteIngredient(Guid id, AppDbContext db)
    {
        var ingredient = await db.Ingredients.Where(i => i.Id == id).FirstOrDefaultAsync();
        
        if (ingredient is null) return Results.NotFound();

        var isUsed = await db.RecipeIngredients
    .AnyAsync(ri => ri.IngredientId == id);

        if(isUsed) return Results.Conflict(new
        {
            message = "INgredient is used by recipe"
        });

        db.Ingredients.Remove(ingredient);

        await db.SaveChangesAsync();

        return Results.Ok();   
    }
}