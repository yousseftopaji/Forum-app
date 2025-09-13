namespace Entities;

public class Comment
{
    public int Id { get; set; }
    public required string Body { get; set; }
    public required int UserId { get; set; }
    public required int PostId { get; set; }
}