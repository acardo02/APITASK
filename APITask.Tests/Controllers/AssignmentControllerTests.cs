using APITask.Controllers;
using APITask.DTOs;
using APITask.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace APITask.Tests.Controllers;

public class AssignmentControllerTests
{
    private readonly Mock<IAssignmentService> _assignmentServiceMock = new();

    private AssignmentController CreateController() => new(_assignmentServiceMock.Object);

    [Fact]
    public async Task GetAllTasks_ReturnsOkWithAssignments()
    {
        var assignments = new List<AssignmentResponseDTO>
        {
            CreateAssignmentResponse(1, "First task"),
            CreateAssignmentResponse(2, "Second task")
        };
        _assignmentServiceMock.Setup(s => s.GetAllAssignments()).ReturnsAsync(assignments);

        var result = await CreateController().GetAllTasks();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(assignments, okResult.Value);
        _assignmentServiceMock.Verify(s => s.GetAllAssignments(), Times.Once);
    }

    [Fact]
    public async Task GetTaskById_WhenAssignmentExists_ReturnsOkWithAssignment()
    {
        var assignment = CreateAssignmentResponse(1, "Existing task");
        _assignmentServiceMock.Setup(s => s.GetAssignmentById(1)).ReturnsAsync(assignment);

        var result = await CreateController().GetTaskById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(assignment, okResult.Value);
    }

    [Fact]
    public async Task GetTaskById_WhenAssignmentDoesNotExist_ReturnsNotFoundProblem()
    {
        _assignmentServiceMock
            .Setup(s => s.GetAssignmentById(99))
            .ReturnsAsync((AssignmentResponseDTO?)null);

        var result = await CreateController().GetTaskById(99);

        AssertProblem(result.Result, StatusCodes.Status404NotFound, "Task not found");
    }

    [Fact]
    public async Task CreateAssignment_WithValidData_ReturnsCreatedAssignment()
    {
        var request = new CreateAssigmentDTO("New task", "Task description");
        var assignment = CreateAssignmentResponse(1, request.Title, request.Description);
        _assignmentServiceMock.Setup(s => s.CreateAssignment(request)).ReturnsAsync(assignment);

        var result = await CreateController().CreateAssignment(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(AssignmentController.GetTaskById), createdResult.ActionName);
        Assert.Equal(assignment.Id, createdResult.RouteValues?["id"]);
        Assert.Same(assignment, createdResult.Value);
        _assignmentServiceMock.Verify(s => s.CreateAssignment(request), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAssignment_WithInvalidTitle_ReturnsBadRequestProblem(string title)
    {
        var request = new CreateAssigmentDTO(title, null);

        var result = await CreateController().CreateAssignment(request);

        AssertProblem(result.Result, StatusCodes.Status400BadRequest, "Invalid Request");
        _assignmentServiceMock.Verify(
            s => s.CreateAssignment(It.IsAny<CreateAssigmentDTO>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTask_WhenAssignmentExists_ReturnsNoContent()
    {
        var request = new UpdateAssigmentDTO("Updated task", "Description", true);
        _assignmentServiceMock.Setup(s => s.UpdateAssignment(1, request)).ReturnsAsync(true);

        var result = await CreateController().UpdateTask(1, request);

        Assert.IsType<NoContentResult>(result);
        _assignmentServiceMock.Verify(s => s.UpdateAssignment(1, request), Times.Once);
    }

    [Fact]
    public async Task UpdateTask_WhenAssignmentDoesNotExist_ReturnsNotFoundProblem()
    {
        var request = new UpdateAssigmentDTO("Updated task", null, false);
        _assignmentServiceMock.Setup(s => s.UpdateAssignment(99, request)).ReturnsAsync(false);

        var result = await CreateController().UpdateTask(99, request);

        AssertProblem(result, StatusCodes.Status404NotFound, "Task not found");
    }

    [Fact]
    public async Task UpdateTask_WithEmptyTitle_ReturnsBadRequestProblem()
    {
        var request = new UpdateAssigmentDTO(string.Empty, null, false);

        var result = await CreateController().UpdateTask(1, request);

        AssertProblem(result, StatusCodes.Status400BadRequest, "Invalid Request");
        _assignmentServiceMock.Verify(
            s => s.UpdateAssignment(It.IsAny<int>(), It.IsAny<UpdateAssigmentDTO>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTask_WhenAssignmentExists_ReturnsNoContent()
    {
        _assignmentServiceMock.Setup(s => s.DeleteAssignment(1)).ReturnsAsync(true);

        var result = await CreateController().DeleteTask(1);

        Assert.IsType<NoContentResult>(result);
        _assignmentServiceMock.Verify(s => s.DeleteAssignment(1), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_WhenAssignmentDoesNotExist_ReturnsNotFoundProblem()
    {
        _assignmentServiceMock.Setup(s => s.DeleteAssignment(99)).ReturnsAsync(false);

        var result = await CreateController().DeleteTask(99);

        AssertProblem(result, StatusCodes.Status404NotFound, "Task not found");
    }

    private static AssignmentResponseDTO CreateAssignmentResponse(
        int id,
        string title,
        string? description = null) =>
        new(id, title, description, false, null, DateTime.UtcNow);

    private static void AssertProblem(
        IActionResult? result,
        int expectedStatusCode,
        string expectedTitle)
    {
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(expectedStatusCode, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(expectedStatusCode, problemDetails.Status);
        Assert.Equal(expectedTitle, problemDetails.Title);
    }
}
