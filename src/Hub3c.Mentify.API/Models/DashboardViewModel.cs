namespace Hub3c.Mentify.API.Models
{
    public class DashboardViewModel
    {
        public int Pending { get; set; }
        public int Connected { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int Mentee { get; set; }
        public int Mentor { get; set; }
    }
}
