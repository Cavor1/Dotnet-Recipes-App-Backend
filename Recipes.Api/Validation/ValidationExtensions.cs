using System.ComponentModel.DataAnnotations;
namespace Recipes.Api.Validation;
public static class ValidationExtensions
{
    public static IResult? Validate<T>(this T model) where T : class
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        if (Validator.TryValidateObject(model, context, results, validateAllProperties: true))
            return null;

        // Group by field name; allow multiple errors per field
        var errors = results
            .SelectMany(r => r.MemberNames.DefaultIfEmpty("")
                .Select(m => new { Member = m, r.ErrorMessage }))
            .GroupBy(x => x.Member)
            .ToDictionary(
                g => string.IsNullOrWhiteSpace(g.Key) ? "Error" : g.Key,
                g => g.Select(x => x.ErrorMessage ?? "Invalid value").ToArray()
            );
        
        return Results.ValidationProblem(errors); // 400 with standard format
    }
}