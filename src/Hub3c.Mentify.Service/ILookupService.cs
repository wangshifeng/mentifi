using System.Collections.Generic;
using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface ILookupService
    {
        IEnumerable<LookupModel<int>> GetSubjectPreferences();
        IEnumerable<LookupModel<int>> GetByAttributeName(string attributeName);
        LookupModel<int> GetByAttribute(string attributeName, int attributeValue);
    }
}
