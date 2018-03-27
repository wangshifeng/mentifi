using System;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class UserAddressService : IUserAddressService
    {
        private IUnitOfWork _unitOfWork;
        private IRepository<SystemUser> _systemUserRepository;
        private IRepository<Business> _businessRepository;

        public UserAddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
            _businessRepository = unitOfWork.GetRepository<Business>();
        }

        public void Update(UserAddressModel model)
        {
            var user = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.SystemUserId, include: a => a.Include(b => b.Business));

            if (user == null)
                throw new ApplicationException("System user id is invalid");

            var business = user.Business;

            business.PhysicalCity = model.PhysicalCity;
            business.PhysicalCountry = model.PhysicalCountry;
            business.PhysicalLine1 = model.PhysicalLine1;
            business.PhysicalLine2 = model.PhysicalLine2;
            business.PhysicalLine3 = model.PhysicalLine3;
            business.PhysicalPostCode = model.PhysicalPostCode;
            business.PhysicalState = model.PhysicalState;
            business.PhysicalSuburb = model.PhysicalSuburb;

            business.PostalCity = model.PostalCity;
            business.PostalCountry = model.PostalCountry;
            business.PostalLine1 = model.PostalLine1;
            business.PostalLine2 = model.PostalLine2;
            business.PostalLine3 = model.PostalLine3;
            business.PostalPostCode = model.PostalPostCode;
            business.PostalState = model.PostalState;
            business.PostalSuburb = model.PostalSuburb;

            business.LocationLong = model.Longitude;
            business.LocationLat = model.Latitude;
            business.GoogleLocation = model.GoogleLocation;

            _businessRepository.Update(business);
            _unitOfWork.SaveChanges();
        }
    }
}
