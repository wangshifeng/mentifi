using MongoDB.Bson;

namespace Hub3c.Mentify.MongoRepo.Model
{
    public class Resource 
    {
        public ObjectId Id { get; private set; }
        public string ResourceName { get; set; }        
        public string ResourceValue { get; set; }
    }
}