using System.Collections.Generic;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;
using Constant = Hub3c.Mentify.Service.Models.Constant;

namespace Hub3c.Mentify.Service.Implementations
{
    public class LookupService : ILookupService
    {
        private readonly IRepository<Repository.Models.LookupTypeCode> _lookupTypeCodeRepository;
        public LookupService(IUnitOfWork unitOfWork)
        {
            _lookupTypeCodeRepository = unitOfWork.GetRepository<Repository.Models.LookupTypeCode>();
        }

        public IEnumerable<LookupModel<int>> GetSubjectPreferences()
        {
            return GetByAttributeName(Constant.LookupTypeCode_EduSubject);
        }

        public IEnumerable<LookupModel<int>> GetByAttributeName(string attributeName)
        {
            return _lookupTypeCodeRepository
                .GetPagedList(selector: a => new LookupModel<int>(a.Value, a.AttributeValue),
                    predicate: a => a.AttributeName == attributeName, pageSize: int.MaxValue).Items;
        }

        public LookupModel<int> GetByAttribute(string attributeName, int attributeValue)
        {
            return _lookupTypeCodeRepository
                .GetFirstOrDefault(selector: a => new LookupModel<int>(a.Value, a.AttributeValue),
                    predicate: a => a.AttributeName == attributeName && a.AttributeValue == attributeValue);
        }
    }
}
