using APITask.DTOs;

namespace APITask.Services
{
    public interface IAssignmentService
    {
        Task<AssignmentResponseDTO> CreateAssignment(CreateAssigmentDTO assignmentDto);
    }
}
