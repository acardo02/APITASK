using APITask.DTOs;
using APITask.Services;
using Microsoft.AspNetCore.Mvc;

namespace APITask.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class AssignmentController(IAssignmentService assignmentService) : ControllerBase
    {
        private readonly IAssignmentService _assignmentService = assignmentService;

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

            return Ok(result);
        }
    }
}
