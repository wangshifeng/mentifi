namespace Hub3c.Mentify.Service.Models
{
    public class UserAddressModel
    {
        public int SystemUserId { get; set; }

        public string PostalLine1 { get; set; }
        public string PostalLine2 { get; set; }
        public string PostalLine3 { get; set; }
        public string PostalState { get; set; }
        public string PostalCountry { get; set; }
        public string PostalCity { get; set; }
        public string PostalPostCode { get; set; }
        public string PostalSuburb { get; set; }

        public string PhysicalLine1 { get; set; }
        public string PhysicalLine2 { get; set; }
        public string PhysicalLine3 { get; set; }
        public string PhysicalCity { get; set; }
        public string PhysicalPostCode { get; set; }
        public string PhysicalCountry { get; set; }
        public string PhysicalState { get; set; }
        public string PhysicalSuburb { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string GoogleLocation { get; set; }

    }
}
