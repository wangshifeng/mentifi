using System.Collections.Generic;
using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IGoalService
    {
        IEnumerable<GoalModel> GetBySystemUser(int systemUserId);
        void Create(NewGoalModel model);
        void Edit(EditedGoalModel model);
        void Delete(int goalId, int systemUserId);
    }
}
