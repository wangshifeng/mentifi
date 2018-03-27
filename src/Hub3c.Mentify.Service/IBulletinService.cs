using System.Collections.Generic;
using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IBulletinService
    {
        IEnumerable<BulletinModel> GetPrivate(int systemUserId, string url);
        IEnumerable<BulletinModel> GetPublic(int systemUserId, string url);
        IEnumerable<CommentModel> GetComments(int bulletinId, string url);
    }
}
