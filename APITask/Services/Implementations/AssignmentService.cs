using APITask.Data;
using APITask.DTOs;
using APITask.Entities;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<AssignmentResponseDTO>> GetAllAssignments()
        {
            return await _dbContext.Assignments
                .AsNoTracking()
                .Select(a => new AssignmentResponseDTO(
                    a.Id,
                    a.Title,
                    a.Description,
                    a.IsCompleted,
                    a.CompletedAt,
                    a.CreatedAt
                )).ToListAsync();
        }

        public async Task<AssignmentResponseDTO?> GetAssignmentById(int id)
        {
            return await _dbContext.Assignments
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AssignmentResponseDTO(
                    a.Id,
                    a.Title,
                    a.Description ?? "",
                    a.IsCompleted,
                    a.CompletedAt,
                    a.CreatedAt
                )).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAssignment(int id, UpdateAssigmentDTO assignmentDto)
        {

            var assignment = await _dbContext.Assignments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
            {
                return false;
            }

            assignment.Title = assignmentDto.Title;
            assignment.Description = assignmentDto.Description ?? "";

            if (assignmentDto.IsCompleted && !assignment.IsCompleted)
            {
                assignment.CompletedAt = DateTime.UtcNow;
            }
            else if (!assignmentDto.IsCompleted)
            {
                assignment.CompletedAt = null;
            }

            assignment.IsCompleted = assignmentDto.IsCompleted;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAssignment(int id)
        {
            var assignment = await _dbContext.Assignments.SingleOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
            {
                return false;
            }


            _dbContext.Assignments.Remove(assignment);

            _dbContext.SaveChanges();

            return true;
        }
    }
}
