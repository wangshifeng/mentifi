using System.Collections.Generic;

namespace Hub3c.Mentify.Service.Models
{
    public class GoalModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public LookupModel<int> Probability { get; set; }
        public IEnumerable<GoalProgressModel> ProgressHistories { get; set; }
    }
}
