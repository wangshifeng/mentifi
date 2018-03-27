using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface ISubjectPreferenceService
    {
        void CreateOrUpdate(CreatedEduSubjectPreference model);
    }
}
