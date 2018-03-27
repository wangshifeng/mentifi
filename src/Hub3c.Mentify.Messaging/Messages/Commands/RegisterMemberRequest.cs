namespace Hub3c.Mentify.Messaging.Messages.Commands
{
    public class RegisterMemberRequest
    {
        public string FullName { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string EncryptedPassword { get; set; } = string.Empty;

        public int Title { get; set; }
    }
}
