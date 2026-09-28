namespace Shop.Application.DTOs.ProductFeedbackDTOs;

public class ProductFeedbackCreateDTO
{
    public string Type { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public int? Rating { get; set; }

    public string AuthorName { get; set; } = string.Empty;
}