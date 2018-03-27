using System;
using System.Collections.Generic;
using System.Linq;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class SystemUserDeviceService : ISystemUserDeviceService
    {
        private readonly IRepository<Repository.Models.SystemUserDevice> _systemUserDeviceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SystemUserDeviceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _systemUserDeviceRepository = unitOfWork.GetRepository<Repository.Models.SystemUserDevice>();
        }

        public void Create(SystemUserDeviceModel model)
        {
            var tokens = _systemUserDeviceRepository
                .GetPagedList(predicate: a => a.DeviceToken == model.DeviceToken, pageSize: int.MaxValue).Items;

            if (tokens.Count > 1)
            {
                _systemUserDeviceRepository.Delete(tokens);
            }
            else if (tokens.Count == 1)
            {
                if (tokens.Any(a => a.SystemUserId == model.SystemUserId))
                {
                    return;
                }
                _systemUserDeviceRepository.Delete(tokens);
            }

            _systemUserDeviceRepository.Insert(new Repository.Models.SystemUserDevice
            {
                CreatedDate = DateTime.Now,
                SystemUserId = model.SystemUserId,
                DeviceToken = model.DeviceToken
            });
            _unitOfWork.SaveChanges();
        }

        public IEnumerable<SystemUserDeviceModel> GetBySystemUserId(int systemUserId)
        {
            return _systemUserDeviceRepository.GetPagedList(predicate: a => a.SystemUserId == systemUserId, selector: a =>
                new SystemUserDeviceModel
                {
                    SystemUserId = a.SystemUserId,
                    DeviceToken = a.DeviceToken
                }).Items;
        }

        public void Delete(SystemUserDeviceModel systemUserDeviceModel)
        {
            var deleted = _systemUserDeviceRepository.GetFirstOrDefault(predicate: a =>
                a.SystemUserId == systemUserDeviceModel.SystemUserId &&
                a.DeviceToken == systemUserDeviceModel.DeviceToken);

            if (deleted != null)
            {
                _systemUserDeviceRepository.Delete(deleted);
                _unitOfWork.SaveChanges();
            }
        }
    }
}
