namespace Hub3c.Mentify.Service.Models
{
    public class LookupModel<T>
    {
        public LookupModel(string name, T id)
        {
            Id = id;
            Name = name;
        }
        public T Id { get; set; }
        public string Name { get; set; }
    }
}
