using System.Linq;
using Hub3c.Mentify.Messaging.Messages.Commands;
using Hub3c.Mentify.MongoRepo;
using Hub3c.Mentify.MongoRepo.Migrator;
using Hub3c.Mentify.MongoRepo.Model;

namespace Hub3c.Mentify.Messaging.Handler
{
    public class ResourceHandler
    {
        private readonly IMongoRepository<Resource> _repository;

        public ResourceHandler(IMongoRepository<Resource> repository)
        {
            _repository = repository;
        }

        public async void SaveAllResource(ResourceData data)
        {
            if (await _repository.Count() > 0)
            {
                await _repository.DeleteAll();                
            }
            var resList = data.ResourceContents.Select(a => new Resource()
            {
                ResourceValue = a.Value,
                ResourceName = a.Name
            }).ToList();
            resList.Add(new Resource()
            {
                ResourceName = "BusinessLinkQr",
                ResourceValue = "{0} is connected to your business now"
            });
            resList.Add(new Resource()
            {
                ResourceName = "BusinessLinkEmail",
                ResourceValue = "{0} has sent you business link request"
            });
            resList.Add(new Resource()
            {
                ResourceName = "BusinessJoinEmail",
                ResourceValue = "{0} has accepted your link request"
            });
            resList.Add(new Resource()
            {
                ResourceName = "AssignProjectMessage",
                ResourceValue = "{0} has assigned you a new project"
            });
            resList.Add(new Resource()
            {
                ResourceName = "AssignProjectActivityMessage",
                ResourceValue = "{0} has assigned you a project activity."
            });
            await _repository.Add(resList);
            var mongoMigrator = new Hub3cMongoMigrator(_repository);            
        }
    }
}