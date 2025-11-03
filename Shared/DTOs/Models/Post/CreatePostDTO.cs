using System.Text.Json.Serialization;

namespace DTOs.Models;

public class CreatePostDTO
{
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public int UserId { get; set; }
}

