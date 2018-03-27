using System.Linq;
using Hub3c.Mentify.MongoRepo.Model;

namespace Hub3c.Mentify.MongoRepo.Migrator
{
    public class Hub3cMongoMigrator
    {
        private readonly IMongoRepository<Resource> _resourceRepository;
        public Hub3cMongoMigrator(IMongoRepository<Resource> resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        public void Migrate()
        {
            Down();
            Up();
        }

        protected void Up()
        {
            UpConnectBusinessMessage();
            UpAssignProjectMessage();
            UpAssignProjectActivityMessage();
        }

        protected void Down()
        {
            DownConnectBusinessMessage();
            DownAssignProjectMessage();
            DownAssignProjectActivityMessage();
        }

        protected async void DownConnectBusinessMessage()
        {
            var businessLinkQr = (await _resourceRepository.GetAll(a => a.ResourceName == "BusinessLinkQr")).FirstOrDefault();
            if (businessLinkQr == null) return;
            await _resourceRepository.Delete(businessLinkQr);
            var businessLinkEmail = (await _resourceRepository.GetAll(a => a.ResourceName == "BusinessLinkEmail")).FirstOrDefault();
            if (businessLinkEmail == null) return;
            await _resourceRepository.Delete(businessLinkEmail);
            var businessJoinEmail = (await _resourceRepository.GetAll(a => a.ResourceName == "BusinessJoinEmail")).FirstOrDefault();
            if (businessJoinEmail == null) return;                       
            await _resourceRepository.Delete(businessJoinEmail);
        }

        protected async void UpConnectBusinessMessage()
        {
            var connectBusinessQr = new Resource()
            {
                ResourceName = "BusinessLinkQr",
                ResourceValue = "{0} is connected to your business now"
            };
            await _resourceRepository.Add(connectBusinessQr);
            var businessLinkEmail = new Resource()
            {
                ResourceName = "BusinessLinkEmail",
                ResourceValue = "{0} has sent you business link request"
            };
            await _resourceRepository.Add(businessLinkEmail);
            var businessJoinEmail = new Resource()
            {
                ResourceName = "BusinessJoinEmail",
                ResourceValue = "{0} has accepted your link request"
            };
            await _resourceRepository.Add(businessJoinEmail);
        }

        protected async void DownAssignProjectMessage()
        {
            var assignProjectMessage = (await _resourceRepository.GetAll(a => a.ResourceName == "AssignProjectMessage")).FirstOrDefault();
            if (assignProjectMessage == null) return;
            await _resourceRepository.Delete(assignProjectMessage);
        }

        protected async void UpAssignProjectMessage()
        {
            var assignProjectMessage = new Resource()
            {
                ResourceName = "AssignProjectMessage",
                ResourceValue = "{0} has assigned you a new project"
            };
            await _resourceRepository.Add(assignProjectMessage);
        }

        protected async void UpAssignProjectActivityMessage()
        {
            var assignActivityMessage = new Resource()
            {
                ResourceName = "AssignProjectActivityMessage",
                ResourceValue = "{0} has assigned you a project activity."
            };
            await _resourceRepository.Add(assignActivityMessage);
        }

        protected async void DownAssignProjectActivityMessage()
        {
            var assignActivityMessage = (await _resourceRepository.GetAll(a => a.ResourceName == "AssignProjectActivityMessage")).FirstOrDefault();
            if (assignActivityMessage == null) return;
            await _resourceRepository.Delete(assignActivityMessage);
        }
    }
}
