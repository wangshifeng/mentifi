namespace Hub3c.Mentify.Service.Models
{
    public class UserSettingModel
    {
        public string MentorAlias { get; set; }
        public string MenteeAlias { get; set; }
        public string ProgramName { get; set; }
        public string UniverisityNameAlias { get; set; }
        public int MaxNumberMenteeForMentor { get; set; }
        public int MaxNumberMentorForMentee { get; set; }
        public bool? IsMentorAllowedToSearchMentee { get; set; }
        public int MaxMenteeRequestedSent { get; set; }
        public int RequestedSentCount { get; set; }
        public int ConnectedCount { get; set; }
    }
}
