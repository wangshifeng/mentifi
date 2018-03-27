using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service.Helpers
{
    public static class ProfileModelMapper
    {
        public static ProfileModel ToProfileModel(this SystemUser systemUser, string baseUrl)
        {
            if (systemUser == null)
                return null;

            return new ProfileModel
            {
                FullName = systemUser.FullName,
                Id = systemUser.SystemUserId,
                PhotoUrl = systemUser.ToPhotoUrl(baseUrl)
            };
        }

        public static ProfileModelIncludingEmail ToProfileModelIncludingEmail(this SystemUser systemUser, string baseUrl)
        {
            if (systemUser == null)
                return null;

            return new ProfileModelIncludingEmail
            {
                FullName = systemUser.FullName,
                Id = systemUser.SystemUserId,
                PhotoUrl = systemUser.ToPhotoUrl(baseUrl),
                Email = systemUser.EmailAddress
            };
        }
    }
}
