using Hub3c.Mentify.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hub3c.Mentify.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service
{
    public interface IDashboardService
    {
        Task<IEnumerable<NotificationModel>> Get(int systemUserId, string url);

        Task<string> ResourceMessage(Notification mobileAppNotification, SystemUser createdBy,
            EduUniversity university, SystemUser systemUser, bool isHtmlVersion);

        Task<PagedListModel<NotificationModel>> Get(int systemUserId, string url, int limit, int pageIndex);
    }
}
