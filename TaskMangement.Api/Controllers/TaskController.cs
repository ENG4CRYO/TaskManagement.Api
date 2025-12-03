using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;

namespace TaskManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        } 

        private string? GetCurrentUserId()
        {
            return User.FindFirst("uid")?.Value;
        }

        [HttpGet]
     
        [ProducesResponseType(typeof(IEnumerable<GetTaskResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllTasks()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var tasks = await _taskService.GetAllTasksAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetTaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var task = await _taskService.GetTaskByIdAsync(id, userId);

            if (task == null) return NotFound();

            return Ok(task);
        }

        [HttpPost]
        [ProducesResponseType(typeof(GetTaskResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddTask([FromBody] CreateTaskDto newTask)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdTask = await _taskService.AddTaskAsync(newTask, userId);

            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updatedTask)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            
            var result = await _taskService.UpdateTaskAsync(id, updatedTask, userId);

            if (!result) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

        
            var result = await _taskService.DeleteTaskAsync(id, userId);

            if (!result) return NotFound();

            return NoContent();
        }
    }
}