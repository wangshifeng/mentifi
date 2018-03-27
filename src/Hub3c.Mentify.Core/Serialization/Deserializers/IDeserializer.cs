namespace Hub3c.Mentify.Core.Serialization.Deserializers
{
    public interface IDeserializer
    {
        T Deserialize<T>(string content);
    }
}