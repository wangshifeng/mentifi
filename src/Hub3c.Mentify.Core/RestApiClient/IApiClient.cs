using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hub3c.Mentify.Core.RestApiClient.Authenticators;
using Hub3c.Mentify.Core.Serialization.Deserializers;

namespace Hub3c.Mentify.Core.RestApiClient
{
    public interface IApiClient
    {
        string UserAgent { get; set; }
        int Timeout { get; set; }
        IAuthenticator Authenticator { get; set; }
        Uri BaseUrl { get; set; }
        Encoding Encoding { get; set; }
        IList<Parameter> DefaultParameters { get; }

        IApiResponse<T> Deserialize<T>(IApiResponse response);

        Task<IApiResponse> ExecuteAsync(IApiRequest request,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IApiResponse<T>> ExecuteAsync<T>(IApiRequest request,
            CancellationToken cancellationToken = default(CancellationToken));

        Uri BuildUri(IApiRequest request);
        void AddHandler(string contentType, IDeserializer deserializer);
        void RemoveHandler(string contentType);
        void ClearHandlers();
    }
}