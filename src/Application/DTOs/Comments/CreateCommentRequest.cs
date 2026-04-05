using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Comments;
public record CreateCommentRequest(
    [Required(ErrorMessage = "Content is required.")]
    [StringLength(500, ErrorMessage = "Content must not exceed 500 characters.")]
    [RegularExpression(@"^\S.*\S$", ErrorMessage = "Content must not be empty or whitespace-only.")]
    string Content
);