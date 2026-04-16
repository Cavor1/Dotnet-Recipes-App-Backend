
namespace Recipes.Api.Services;
public class Result<T>
{
    public T? Value {get;init;}
    public string? Error {get;init;}
}
