namespace Hub3c.Mentify.Messaging.Messages.Commands
{
    public class ApproveContractorToProject
    {
        public int SystemUserId { get; set; }
        public int BusinessId { get; set; }
        public int ProjectId { get; set; }
        public int ContractorId { get; set; }
    }
}
