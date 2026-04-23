
using Microsoft.EntityFrameworkCore;
using Recipes.Api.Data;
using Recipes.Api.Dto;
using Recipes.Api.Entities;
using Recipes.Api.Validation;
using Recipes.Api.Services;
using System.Reflection.Metadata.Ecma335;

namespace Recipes.Api.Endpoints;

public static class StatsEndpoints
{
    public static void MapStatsEndpoints(this WebApplication app)
    {
        app.MapGet("/stats", GetStats);
        app.MapGet("/stats/today", GetTodayStats);
    }

    public static async Task<IResult> GetStats([AsParameters] GetStatsQueryDto req ,AppDbContext db)
    {
        var reqValidation = req.Validate();
        if (reqValidation is not null) return reqValidation;

        TimeZoneInfo tz;
        try
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById(req.TimeZone ?? "UTC");
        }
        catch (TimeZoneNotFoundException)
        {
            return Results.BadRequest("invalid timezone");    
        }

        var query = db.Meals.AsQueryable();

        var fromUtc = StatsService.GetDayStartUtc(req.From, tz);
        var toUtc = StatsService.GetDayStartUtc(req.To.AddDays(1), tz);

        var items = await db.MealIngredients
            .Where(mi =>
                mi.Meal.EatenTime != null &&
                mi.Meal.EatenTime >= fromUtc &&
                mi.Meal.EatenTime < toUtc)
            .Select(mi => new
            {
                EatenTime = mi.Meal.EatenTime!.Value,
                Kcal = mi.Gram * (mi.Ingredient.Kcal100g ?? 0) / 100.0
            })
            .ToListAsync();

        var daily = items
            .GroupBy(x => TimeZoneInfo.ConvertTimeFromUtc(x.EatenTime, tz).Date)
            .Select(g => new DailyStatsDto
            {
                Date = DateOnly.FromDateTime(g.Key),
                Kcal = g.Sum(x => x.Kcal) ?? 0
            })
            .OrderBy(x => x.Date)
            .ToList();
        return Results.Ok(daily);
    } 

    public static async Task<IResult> GetTodayStats([AsParameters] GetTodayStatsQueryDto req, AppDbContext db)
    {

        var reqValidation = req.Validate();
        if (reqValidation is not null) return reqValidation;

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