using Microsoft.EntityFrameworkCore;
using Recipes.Api.Data;
using Recipes.Api.Dto;
using Recipes.Api.Entities;

namespace Recipes.Api.Services;

public class StatsService
{
    public static DateTime GetDayStartUtc(DateOnly date, TimeZoneInfo tz)
    {
        var datetime = date.ToDateTime(TimeOnly.MinValue);
        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(datetime,tz);
        return utcDateTime; 
    }
}