using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hub3c.Mentify.Service.Implementations;
using Hub3c.ApiMessage;
using Hub3c.Mentify.AccessInternalApi;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Hub3c.Mentify.ServiceTest
{
    public class ConnectionServiceTest
    {
        [Fact]
        public async Task RequestConnection_Resource()
        {
            var businessToBusinesses = new List<BusinessToBusiness>();

            var list = new List<SystemUser>()
            {
                new SystemUser()
                {
                    SystemUserId = 1,
                    BusinessId = 1,
                    Business = new Business()
                    {
                        EduBusinessType = 1,
                        UniversityId = 1,
                        BusinessId = 1,
                        BusinessToBusiness = businessToBusinesses
                    }
                },
                new SystemUser()
                {
                    SystemUserId = 2,
                    Business = new Business()
                    {
                        EduBusinessType = 2,
                        UniversityId = 1,
                        BusinessId = 2,
                        BusinessToBusiness = businessToBusinesses
                    },
                    BusinessId = 2
                }
            };

            var eduUniversities = new List<EduUniversity>()
            {
                new EduUniversity
                {
                    BusinessId = 1,
                    MaxMenteeRequest = 3,
                    MaxNumberMentorForMentee = 3,
                    MaxNumberMenteeForMentor = 3,
                    MenteeAlias = "Mentee",
                    MentorAlias = "Mentor",
                    IsMentorAllowedToSearchMentee = true,
                    ProgramName = "Program",
                    UniversityNameAlias = "University"
                }
            };

            var applicationUsages = new List<MentifiApplicationUsage>();

            var contex = new Mock<InMemoryContext>();
            var systemUser = DbSetMock.Get(list);
            contex.Setup(a => a.Set<SystemUser>()).Returns(systemUser.Object);


            var applicationUsage = DbSetMock.Get(applicationUsages);

            contex.Setup(a => a.Set<MentifiApplicationUsage>()).Returns(applicationUsage.Object);

            var eduUniversity = DbSetMock.Get(eduUniversities);
            contex.Setup(a => a.Set<EduUniversity>()).Returns(eduUniversity.Object);


            var notifications = new List<Notification>();
            var notification = DbSetMock.Get(notifications);
            contex.Setup(a => a.Set<Notification>()).Returns(notification.Object);
            var businessToBusiness = DbSetMock.Get(businessToBusinesses);
            contex.Setup(a => a.Set<BusinessToBusiness>()).Returns(businessToBusiness.Object);
            var lookupService = new Mock<ILookupService>().Object;
            var messageService = new Mock<MessageBoardService>().Object;
            var emailApi = new Mock<IEmailApi>().Object;
            var configuration = new Mock<IConfiguration>().Object;
            var busInstance = new Mock<IBusInstance>().Object;

            var service = new ConnectionService(new UnitOfWork<InMemoryContext>(contex.Object), lookupService, emailApi, configuration, busInstance, messageService);

            await service.Request(1, 2, "WOW");

            Assert.True(applicationUsages.Count == 2);
            Assert.True(businessToBusinesses.Count == 2);
            Assert.True(notifications.Count == 1);
        }
    }
}