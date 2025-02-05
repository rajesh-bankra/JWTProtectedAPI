using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkerServiceDemo;

namespace JWTProtectedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackgroundJobController : ControllerBase
    {
        private readonly IBackgroundTaskQueue _taskQueue;

        public BackgroundJobController(IBackgroundTaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        [HttpGet("start-task")]
        public async Task<IActionResult> StartBackgroundTask()
        {
            // Enqueue a task
            await _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                // Simulate long-running work
                await Task.Delay(5000, token);
            });

            return Ok("Task started.");
        }
    }
}
