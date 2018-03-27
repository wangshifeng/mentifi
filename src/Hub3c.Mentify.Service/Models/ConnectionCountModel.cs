namespace Hub3c.Mentify.Service.Models
{
    public class ConnectionCountModel
    {
        public int Requested { get; set; }
        public int Connected { get; internal set; }
        public int Pending { get; set; }
    }

    public class AdminConnectionCountModel
    {
        public int Mentee { get; set; }
        public int Mentor { get; internal set; }
    }
}
