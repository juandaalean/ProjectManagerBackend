namespace Application.DTOs.Comments;
public record CommentDto(
    Guid CommentId,
    Guid TaskId,
    Guid UserId,
    string UserName,
    string Content,
    DateTime CreateAt
);