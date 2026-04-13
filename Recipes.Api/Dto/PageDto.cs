namespace Recipes.Api.Dto;

public class PageMetadataDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class PagedResponseDto<T>
{
    public List<T> Items { get; set; } = new();
    public PageMetadataDto Metadata { get; set; } = new();
}