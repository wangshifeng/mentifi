using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.API.Models;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Tasks")]
    [ApiVersion("1")]
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;

        /// <inheritdoc />
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        ///// <summary>
        ///// Get Pending Tasks
        ///// </summary>
        ///// <param name="fromSystemUserId"></param>
        ///// <param name="toSystemUserId"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("Pending")]
        //public IActionResult Pending([FromQuery]int fromSystemUserId, [FromQuery]int toSystemUserId)
        //{
        //    var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
        //    var goals = _taskService.GetPendingByAssignedToSystemUserIdAndSystemUserId(fromSystemUserId, toSystemUserId, baseUrl);
        //    return Ok(MessageHelper.Success(goals));
        //}

        /// <summary>
        /// Get Pending Tasks
        /// </summary>
        /// <param name="fromSystemUserId"></param>
        /// <param name="toSystemUserId"></param>
        ///  /// <param name="limit"></param>
        /// <param name="pageIndex">start from 0</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Pending")]
        public IActionResult Pending([FromQuery]int fromSystemUserId, [FromQuery]int toSystemUserId, [FromQuery] int limit, [FromQuery] int pageIndex)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            var goals = _taskService.GetPendingByAssignedToSystemUserIdAndSystemUserId(fromSystemUserId, toSystemUserId, baseUrl, pageIndex, limit);
            return Ok(MessageHelper.Success(goals));
        }

        ///// <summary>
        ///// Get Completed Tasks
        ///// </summary>
        ///// <param name="fromSystemUserId"></param>
        ///// <param name="toSystemUserId"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("Completed")]
        //public IActionResult Compeleted([FromQuery]int fromSystemUserId, [FromQuery]int toSystemUserId)
        //{
        //    var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
        //    var goals = _taskService.GetCompletedByAssignedToSystemUserIdAndSystemUserId(fromSystemUserId, toSystemUserId, baseUrl);
        //    return Ok(MessageHelper.Success(goals));
        //}

        /// <summary>
        /// Get Completed Tasks
        /// </summary>
        /// <param name="fromSystemUserId"></param>
        /// <param name="toSystemUserId"></param>
        /// <param name="limit"></param>
        /// <param name="pageIndex">start from 0</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Completed")]
        public IActionResult Compeleted([FromQuery]int fromSystemUserId, [FromQuery]int toSystemUserId, [FromQuery] int limit, [FromQuery] int pageIndex)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            var goals = _taskService.GetCompletedByAssignedToSystemUserIdAndSystemUserId(fromSystemUserId, toSystemUserId, baseUrl, pageIndex, limit);
            return Ok(MessageHelper.Success(goals));
        }

        /// <summary>
        /// RequestConnection a Task
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody]NewTaskModel model)
        {
            _taskService.Create(model);
            return Ok(MessageHelper.Success("The task has been created successfully."));
        }

        /// <summary>
        /// Edit a Task
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{taskId}")]
        public IActionResult Put([FromRoute] int taskId, [FromBody]EditedTaskViewModel model)
        {
            _taskService.Edit(new EditedTaskModel()
            {
                Id = taskId,
                Status = model.Status,
                ModifiedBySystemUserId = model.ModifiedBySystemUserId,
                DueDate = model.DueDate,
                Priority = model.Priority,
                AssignedToSystemUserId = model.AssignedToSystemUserId,
                Subject = model.Subject
            });
            return Ok(MessageHelper.Success("The task has been updated successfully."));
        }

        /// <summary>
        /// Assign a Task
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{taskId}/Assignee")]
        public IActionResult Assign([FromRoute] int taskId, [FromBody]AssigningTaskViewModel model)
        {
            _taskService.Assign(new TaskAssigneeModel
            {
                AssignedToSystemUserId = model.AssignedToSystemUserId,
                Id = taskId,
                ModifiedBySystemUserId = model.ModifiedBySystemUserId
            });
            return Ok(MessageHelper.Success("The task has been created successfully."));
        }

        /// <summary>
        /// Delete a Task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{taskId}")]
        public IActionResult Delete([FromRoute] int taskId)
        {
            _taskService.Delete(taskId);
            return Ok(MessageHelper.Success("The task has been deleted successfully."));
        }

        /// <summary>
        /// Change  Task Status
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{taskId}/Status")]
        public IActionResult Status([FromRoute] int taskId, [FromBody]TaskStatusViewModel model)
        {
            _taskService.ChangeStatus(new TaskStatusModel
            {
                Id = taskId,
                Status = model.Status,
                ModifiedBySystemUserId = model.ModifiedBySystemUserId
            });
            return Ok(MessageHelper.Success("The task status has been updated successfully."));
        }

        ///// <summary>
        ///// Get My Tasks
        ///// </summary>
        ///// <param name="assignTo">System User Id</param>
        ///// <param name="isCompleted"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("Asignee")]
        //public IActionResult MyTask([FromQuery]int assignTo, [FromQuery]bool isCompleted)
        //{
        //    var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
        //    var goals = _taskService.GetMyTask(assignTo, baseUrl, isCompleted);
        //    return Ok(MessageHelper.Success(goals));
        //}

        /// <summary>
        /// Get Assigned Tasks
        /// </summary>
        /// <param name="assignTo">System User Id</param>
        /// <param name="isCompleted"></param>
        /// <param name="limit"></param>
        /// <param name="pageIndex">start from 0</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Asignee")]
        public IActionResult MyTask([FromQuery]int assignTo, [FromQuery]bool isCompleted, [FromQuery] int limit, [FromQuery] int pageIndex)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            var goals = _taskService.GetMyTask(assignTo, baseUrl, pageIndex, limit, isCompleted);
            return Ok(MessageHelper.Success(goals));
        }
    }

    ///// <inheritdoc />
    //[Produces("application/json")]
    //[Route("api/v{version:apiVersion}/Tasks")]
    //[ApiVersion("2")]
    //[Authorize]
    //public class TasksV2Controller : Controller
    //{

    //    private readonly ITaskService _taskService;

    //    /// <inheritdoc />
    //    public TasksV2Controller(ITaskService taskService)
    //    {
    //        _taskService = taskService;
    //    }

    //    /// <summary>
    //    /// Get Pending Tasks
    //    /// </summary>
    //    /// <param name="fromSystemUserId"></param>
    //    /// <param name="toSystemUserId"></param>
    //    ///  /// <param name="limit"></param>
    //    /// <param name="pageIndex">start from 0</param>
    //    /// <returns></returns>
    //    [HttpGet]
    //    [Route("Pending")]
    //    public IActionResult Pending([FromQuery]int fromSystemUserId, [FromQuery]int toSystemUserId, [FromQuery] int limit, [FromQuery] int pageIndex)
    //    {
    //        var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
    //        var goals = _taskService.GetPendingByAssignedToSystemUserIdAndSystemUserId(fromSystemUserId, toSystemUserId, baseUrl, pageIndex, limit);
    //        return Ok(MessageHelper.Success(goals));
    //    }

    //    /// <summary>
    //    /// Get Completed Tasks
    //    /// </summary>
    //    /// <param name="fromSystemUserId"></param>
    //    /// <param name="toSystemUserId"></param>
    //    /// <param name="limit"></param>
    //    /// <param name="pageIndex">start from 0</param>
    //    /// <returns></returns>
    //    [HttpGet]
    //    [Route("Completed")]
    //    public IActionResult Compeleted([FromQuery]int fromSystemUserId, [FromQuery]int toSystemUserId, [FromQuery] int limit, [FromQuery] int pageIndex)
    //    {
    //        var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
    //        var goals = _taskService.GetCompletedByAssignedToSystemUserIdAndSystemUserId(fromSystemUserId, toSystemUserId, baseUrl, pageIndex, limit);
    //        return Ok(MessageHelper.Success(goals));
    //    }

    //    /// <summary>
    //    /// Get Assigned Tasks
    //    /// </summary>
    //    /// <param name="assignTo">System User Id</param>
    //    /// <param name="isCompleted"></param>
    //    /// <param name="limit"></param>
    //    /// <param name="pageIndex">start from 0</param>
    //    /// <returns></returns>
    //    [HttpGet]
    //    [Route("Asignee")]
    //    public IActionResult MyTask([FromQuery]int assignTo, [FromQuery]bool isCompleted, [FromQuery] int limit, [FromQuery] int pageIndex)
    //    {
    //        var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
    //        var goals = _taskService.GetMyTask(assignTo, baseUrl, pageIndex, limit, isCompleted);
    //        return Ok(MessageHelper.Success(goals));
    //    }
    //}
}