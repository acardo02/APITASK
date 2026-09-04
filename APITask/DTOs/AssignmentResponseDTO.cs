namespace APITask.DTOs
{
    public record AssignmentResponseDTO
    (
        int Id,

        string Title,

        string? Description,

        bool IsCompleted,

        DateTime? CompletedAt,

        DateTime CreatedAt
    );
}
