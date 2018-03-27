using Hub3c.Mentify.Repository.Models;

namespace Hub3c.Mentify.Service.Helpers
{
    public static class UrlHelper
    {
        public static string ToPhotoUrl(this SystemUser systemUser, string baseUrl)
        {
            return systemUser.ProfilePhoto != null ? $"{baseUrl}/Photos/{systemUser.SystemUserId}" : string.Empty;
        }

        public static string ToResumeDocumentUrl(this EduUser eduUser, string baseUrl)
        {
            return eduUser?.ResumeDocumentId != null ? $"{baseUrl}/Documents/{eduUser.ResumeDocumentId}" : string.Empty;
        }

        public static string ToPlanDocumentUrl(this EduUser eduUser, string baseUrl)
        {
            return eduUser?.PlanDocumentId != null ? $"{baseUrl}/Documents/{eduUser.PlanDocumentId}" : string.Empty;
        }

        public static string ToDocumentUrl(this int id, string baseUrl)
        {
            return $"{baseUrl}/Documents/{id}";
        }
    }
}
