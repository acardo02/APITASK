using APITask.Data;
using APITask.DTOs;
using APITask.Entities;

namespace APITask.Services.Implementations
{
    public class AssignmentService(AppDbContext dbContext) : IAssignmentService
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<AssignmentResponseDTO> CreateAssignment(CreateAssigmentDTO assignmentDto)
        {
            var assignment = new Assignment
            {
                Title = assignmentDto.Title,
                Description = assignmentDto.Description ?? "",
                IsCompleted = false,
                CompletedAt = null,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Assignments.Add(assignment);
            await _dbContext.SaveChangesAsync();

            return new AssignmentResponseDTO(
                assignment.Id,
                assignment.Title,
                assignment.Description,
                assignment.IsCompleted,
                assignment.CompletedAt,
                assignment.CreatedAt
            );
        }
    }
}
