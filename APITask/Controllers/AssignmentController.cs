using APITask.DTOs;
using APITask.Services;
using APITask.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace APITask.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class AssignmentController(IAssignmentService assignmentService) : ControllerBase
    {
        private readonly IAssignmentService _assignmentService = assignmentService;

        [HttpGet]
        [ProducesResponseType(typeof(List<AssignmentResponseDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<AssignmentResponseDTO>> GetAllTasks()
        {
            var assignments = await _assignmentService.GetAllAssignments();

            return Ok(assignments);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AssignmentResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AssignmentResponseDTO>> GetTaskById(int id)
        {
            var assignment = await _assignmentService.GetAssignmentById(id);

            if (assignment is null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Task not found"
                );
            }

            return Ok(assignment);
        }

        [HttpPost]
        [ProducesResponseType(typeof(AssignmentResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AssignmentResponseDTO>> CreateAssignment(CreateAssigmentDTO createAssigmentDTO)
        {
            if (string.IsNullOrWhiteSpace(createAssigmentDTO.Title))
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid Request",
                    detail: "The title can't be empty"
                );
            }

            var result = await _assignmentService.CreateAssignment(createAssigmentDTO);

            return CreatedAtAction(nameof(GetTaskById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateTask(int id, UpdateAssigmentDTO updateAssigmentDTO)
        {
            if (string.IsNullOrEmpty(updateAssigmentDTO.Title))
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid Request",
                    detail: "The title can't be empty"
                );
            }

            var updated = await _assignmentService.UpdateAssignment(id, updateAssigmentDTO);

            if (!updated)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Task not found"
                );
            }

            return NoContent();
        }
    }
}
