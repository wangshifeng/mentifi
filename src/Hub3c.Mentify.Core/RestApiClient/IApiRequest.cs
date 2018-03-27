using System.Collections.Generic;

namespace Hub3c.Mentify.Core.RestApiClient
{
    public interface IApiRequest
    {
        IList<Parameter> Parameters { get; }
        RequestMethod Method { get; set; }
        string Resource { get; set; }
        DataFormat RequestFormat { get; set; }
        int Timeout { get; set; }
        int Attempts { get; }

        IApiRequest AddBody(object obj);
        IApiRequest AddParameter(Parameter p);
        IApiRequest AddParameter(string name, object value, ParameterType type);
        IApiRequest AddOrUpdateParameter(Parameter p);
        IApiRequest AddOrUpdateParameter(string name, object value, ParameterType type);
        void IncreaseNumAttempts();
    }
}