using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IUserProfileService
    {
        CurrentUser Get(int mid, string baseUrl);
        UserProfileModel GetBySystemUserId(int systemUserId, string url);
        UserSettingModel GetUserSetting(int systemUserId);
        LookupModel<int> GetMentifiType(int businessId);
        void  Edit(EditedUserProfileModel model);
        void  EditBiography(EditedBiography model);
        void  EditPersonalGoal(EditedPersonalGoal model);
        byte[] GetPhoto(int id);
    }
}