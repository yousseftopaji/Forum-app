namespace DTOs.Models;

public class CreatePostDTO
{
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string AuthorUserId { get; set; } = null!;
}

