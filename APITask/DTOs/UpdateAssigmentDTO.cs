namespace APITask.DTOs
{
    public record UpdateAssigmentDTO(

        string Title,
        string? Description,
        bool IsCompleted
    );
}
