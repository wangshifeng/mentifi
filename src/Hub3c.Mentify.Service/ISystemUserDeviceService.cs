using System.Collections.Generic;
using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface ISystemUserDeviceService
    {
        void Create(SystemUserDeviceModel model);
        IEnumerable<SystemUserDeviceModel> GetBySystemUserId(int systemUserId);
        void Delete(SystemUserDeviceModel systemUserDeviceModel);
    }
}
