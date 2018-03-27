namespace Hub3c.Mentify.Core.RestApiClient
{
    public class Parameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ParameterType Type { get; set; }
        public string ContentType { get; set; }

        public override string ToString()
        {
            return $"{Name}={Value}";
        }
    }

    public enum ParameterType
    {
        Cookie,
        GetOrPost,
        UrlSegment,
        HttpHeader,
        RequestBody,
        QueryString
    }
}