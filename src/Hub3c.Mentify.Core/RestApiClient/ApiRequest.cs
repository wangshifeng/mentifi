using System;
using System.Collections.Generic;
using System.Linq;
using Hub3c.Mentify.Core.Serialization.Serializers;

namespace Hub3c.Mentify.Core.RestApiClient
{
    public class ApiRequest : IApiRequest
    {
        private readonly FormUrlEncodedSerializer _formUrlEncodedSerializer = new FormUrlEncodedSerializer();

        private readonly JsonSerializer _jsonSerializer = new JsonSerializer();
        private readonly XmlSerializer _xmlSerializer = new XmlSerializer();

        public ApiRequest()
        {
            RequestFormat = DataFormat.Json;
            Method = RequestMethod.GET;
            Parameters = new List<Parameter>();
        }

        public ApiRequest(RequestMethod method) : this()
        {
            Method = method;
        }

        public ApiRequest(string resource, RequestMethod method) : this()
        {
            Resource = resource;
            Method = method;
        }

        public ApiRequest(string resource) : this(resource, RequestMethod.GET)
        {
        }

        public ApiRequest(Uri resource, RequestMethod method) : this(
            resource.IsAbsoluteUri ? resource.AbsolutePath + resource.Query : resource.OriginalString, method)
        {
        }

        public ApiRequest(Uri resource) : this(resource, RequestMethod.GET)
        {
        }

        public IList<Parameter> Parameters { get; }
        public RequestMethod Method { get; set; }
        public string Resource { get; set; }
        public DataFormat RequestFormat { get; set; }
        public int Timeout { get; set; }
        public int Attempts { get; private set; }

        public IApiRequest AddBody(object obj)
        {
            string serialized;
            string contentType;

            switch (RequestFormat)
            {
                case DataFormat.Json:
                    serialized = _jsonSerializer.Serialize(obj);
                    contentType = "application/json";
                    break;
                case DataFormat.Xml:
                    serialized = _xmlSerializer.Serialize(obj);
                    contentType = "text/xml";
                    break;
                case DataFormat.FormUrlEncoded:
                    serialized = _formUrlEncodedSerializer.Serialize(obj);
                    contentType = "application/x-www-form-urlencoded";
                    break;
                default:
                    serialized = string.Empty;
                    contentType = string.Empty;
                    break;
            }

            return AddParameter(
                new Parameter {Name = contentType, Value = serialized, Type = ParameterType.RequestBody});
        }

        public IApiRequest AddParameter(Parameter p)
        {
            Parameters.Add(p);
            return this;
        }

        public IApiRequest AddParameter(string name, object value, ParameterType type)
        {
            return AddParameter(new Parameter {Name = name, Value = value, Type = type});
        }

        public IApiRequest AddOrUpdateParameter(Parameter p)
        {
            if (Parameters.Any(param => param.Name == p.Name))
            {
                var parameter = Parameters.First(param => param.Name == p.Name);
                parameter.Value = p.Value;
                return this;
            }

            Parameters.Add(p);
            return this;
        }

        public IApiRequest AddOrUpdateParameter(string name, object value, ParameterType type)
        {
            return AddOrUpdateParameter(new Parameter {Name = name, Value = value, Type = type});
        }

        public void IncreaseNumAttempts()
        {
            Attempts++;
        }
    }
}