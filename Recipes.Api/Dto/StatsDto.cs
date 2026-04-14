
namespace Recipes.Api.Dto;
using System.ComponentModel.DataAnnotations;

public class GetStatsQueryDto
{
    public DateOnly? From {get;set;}
    public DateOnly? To {get;set;}
    public string? TimeZone {get;set;}
}

public class GetTodayStatsQueryDto
{
    public string? TimeZone {get;set;}
}