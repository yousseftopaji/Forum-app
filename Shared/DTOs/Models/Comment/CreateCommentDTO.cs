namespace DTOs.Models;

public class CreateCommentDTO
{
    public required string Body { get; set; }
    public required int UserId { get; set; }
    public required int PostId { get; set; }
}