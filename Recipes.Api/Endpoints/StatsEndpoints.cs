
using Microsoft.EntityFrameworkCore;
using Recipes.Api.Data;
using Recipes.Api.Dto;
using Recipes.Api.Entities;
using Recipes.Api.Validation;

namespace Recipes.Api.Endpoints;

public static class StatsEndpoints
{
    public static void MapIngredientsEndpoints(this WebApplication app)
    {
        app.MapGet("/stats", GetStats);
        app.MapGet("/stats/today", GetTodayStats);
    }

    public static async Task<IResult> GetStats([AsParameters] GetStatsQueryDto req ,AppDbContext db)
    {
        return Results.StatusCode(501);
    }

    public static async Task<IResult> GetTodayStats([AsParameters] GetTodayStatsQueryDto req, AppDbContext db)
    {
        var query = db.Meals.AsQueryable();
        TimeZoneInfo tz;
        try
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById(req.TimeZone ?? "UTC");
        }
        catch (TimeZoneNotFoundException)
        {
            return Results.BadRequest("invalid timezone");    
        }

        var nowInTz = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);

        var startOfDayLocal = nowInTz.Date;
        var endOfDayLocal = startOfDayLocal.AddDays(1);

        var startUtc = TimeZoneInfo.ConvertTimeToUtc(startOfDayLocal, tz);
        var endUtc = TimeZoneInfo.ConvertTimeToUtc(endOfDayLocal, tz);

        query = query.Where(m =>m.EatenTime != null && m.EatenTime >= startUtc && m.EatenTime < endUtc);

        var kcal = await query
            .SumAsync(m => m.MealIngredients
                .Sum(mi => mi.Gram*mi.Ingredient.Kcal100g/100) ?? 0);
        return Results.Ok(kcal);
    }
}