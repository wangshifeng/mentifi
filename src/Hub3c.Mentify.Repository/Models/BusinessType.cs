using System.Collections.Generic;

namespace Hub3c.Mentify.Repository.Models
{
    public class BusinessType 
    {
        public BusinessType()
        {
            Business = new HashSet<Business>();
        }

        public int BusinessTypeId { get; set; }
        public string TypeName { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Business> Business { get; set; }
    }
}