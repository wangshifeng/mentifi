using Hub3c.Mentify.Repository.Models;

namespace Hub3c.Mentify.API.Models
{
    public class EditedGoalViewModel
    {
        public string Description { get; set; }
        public int ProbabilityId { get; set; }
        public int MenteeSystemUserId { get; set; }
    }
}
