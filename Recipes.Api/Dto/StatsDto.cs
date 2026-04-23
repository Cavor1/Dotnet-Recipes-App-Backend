
namespace Recipes.Api.Dto;
using System.ComponentModel.DataAnnotations;
using Microsoft.Net.Http.Headers;

public class GetStatsQueryDto
{
    public DateOnly From {get;set;}
    public DateOnly To {get;set;}
    public string? TimeZone {get;set;}
}

public class GetTodayStatsQueryDto
{
    public string? TimeZone {get;set;}
}

public class DailyStatsDto
{
    public DateOnly Date {get;set;}
    public double Kcal {get;set;}
}