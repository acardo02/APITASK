using APITask.DTOs;

namespace APITask.Services
{
    public interface IAssignmentService
    {
        Task<AssignmentResponseDTO> CreateAssignment(CreateAssigmentDTO assignmentDto);

        Task<IEnumerable<AssignmentResponseDTO>> GetAllAssignments();

        Task<AssignmentResponseDTO?> GetAssignmentById(int id);

        Task<bool> UpdateAssignment(int id, UpdateAssigmentDTO assignmentDto);
    }
}
