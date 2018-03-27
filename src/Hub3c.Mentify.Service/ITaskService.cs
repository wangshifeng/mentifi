using System.Collections.Generic;
using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface ITaskService
    {
        IEnumerable<TaskModel> GetPendingByAssignedToSystemUserIdAndSystemUserId(int fromSystemUserId,
            int toSystemUserId, string baseUrl);

        IEnumerable<TaskModel> GetCompletedByAssignedToSystemUserIdAndSystemUserId(int fromSystemUserId,
            int toSystemUserId, string baseUrl);

        PagedListModel<TaskModel> GetPendingByAssignedToSystemUserIdAndSystemUserId(int fromSystemUserId,
            int toSystemUserId, string baseUrl, int pageIndex, int limit);

        PagedListModel<TaskModel> GetMyTask(int systemUserId, string baseUrl, int pageIndex, int limit,
            bool isCompleted);

        IEnumerable<TaskModel> GetMyTask(int systemUserId, string baseUrl, bool isCompleted);

        PagedListModel<TaskModel> GetCompletedByAssignedToSystemUserIdAndSystemUserId(int fromSystemUserId,
            int toSystemUserId, string baseUrl, int pageIndex, int limit);

        void Create(NewTaskModel model);

        void Edit(EditedTaskModel model);

        void Assign(TaskAssigneeModel model);

        void Delete(int taskId);

        void ChangeStatus(TaskStatusModel model);
    }
}
