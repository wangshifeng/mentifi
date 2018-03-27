using System;
using System.Threading.Tasks;
using Hub3c.Mentify.AccessInternalApi.Models;
using Hub3c.Mentify.Core.RestApiClient;
using Hub3c.Mentify.Core.RestApiClient.Authenticators;
using Hub3c.Mentify.Core.Serialization.Serializers;
using Microsoft.Extensions.Configuration;

namespace Hub3c.Mentify.AccessInternalApi.Implementations
{
    public class Hub3CFirebaseApi : IHub3cFirebaseApi
    {
        private readonly IApiClient _apiClient;
        private readonly IConfiguration _config;
        public Hub3CFirebaseApi(IApiClient apiClient, IConfiguration config)
        {
            _apiClient = apiClient;
            _config = config;
        }

        public async Task<object> Send(Hub3cFirebase model)
        {
            _apiClient.BaseUrl = new Uri(_config.GetSection("Hub3cFirebase")["Url"]);
            var apiRequest = new ApiRequest("", RequestMethod.POST) { RequestFormat = DataFormat.Json };
            _apiClient.Authenticator = new FcmAuthenticator(_config.GetSection("Hub3cFirebase")["Token"]);
            apiRequest.AddBody(model);
            var resultAsync = await _apiClient.ExecuteAsync<object>(apiRequest);
            if (resultAsync.IsSuccessful)
                return resultAsync.Data;

            throw new ApplicationException(resultAsync.ErrorMessage);
        }
    }
}
