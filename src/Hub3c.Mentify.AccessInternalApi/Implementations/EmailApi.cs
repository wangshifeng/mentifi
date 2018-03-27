using System;
using System.Threading.Tasks;
using Hub3c.Mentify.AccessInternalApi.Models;
using Hub3c.Mentify.Core.RestApiClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hub3c.Mentify.AccessInternalApi.Implementations
{
    public class EmailApi : IEmailApi
    {
        private readonly IApiClient _apiClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public EmailApi(IApiClient apiClient, IConfiguration configuration, ILogger<EmailApi> logger)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<object> RequestConnection(EmailParam model)
        {
            _apiClient.BaseUrl = new Uri(_configuration["EmailServiceUrl"]);

            var apiRequest = new ApiRequest("", RequestMethod.POST) { RequestFormat = DataFormat.Json };
            apiRequest.AddBody(new EmailApiModel()
            {
                EmailType = "EduHubConnection",
                SystemUserId = model.SystemUserId,
                Payload = model.Payload,
                Recipient = model.Recipient,
                TemplateName = "Mentifi_ConnectRequest"
            });
            var response = await _apiClient.ExecuteAsync<object>(apiRequest);

            if (response.IsSuccessful)
            {
                return model;
            }
            _logger.LogError(response.ErrorException, response.ErrorMessage, null);
            throw new ApplicationException(response.ErrorMessage);
        }

        public async Task<object> RejectConnection(EmailParam model)
        {
            _apiClient.BaseUrl = new Uri(_configuration["EmailServiceUrl"]);

            var apiRequest = new ApiRequest("", RequestMethod.POST) { RequestFormat = DataFormat.Json };
            apiRequest.AddBody(new EmailApiModel()
            {
                EmailType = "EduHubRejectInvitation",
                SystemUserId = model.SystemUserId,
                Payload = model.Payload,
                Recipient = model.Recipient,
                TemplateName = "Mentifi_RejectInvitation"
            });
            var response = await _apiClient.ExecuteAsync<object>(apiRequest);

            if (response.IsSuccessful)
            {
                return model;
            }
            _logger.LogError(response.ErrorException, response.ErrorMessage, null);
            throw new ApplicationException(response.ErrorMessage);
        }

        public async Task<object> AcceptConnection(EmailParam model)
        {
            _apiClient.BaseUrl = new Uri(_configuration["EmailServiceUrl"]);

            var apiRequest = new ApiRequest("", RequestMethod.POST) { RequestFormat = DataFormat.Json };
            apiRequest.AddBody(new EmailApiModel()
            {
                EmailType = "EduHubConnection",
                SystemUserId = model.SystemUserId,
                Payload = model.Payload,
                Recipient = model.Recipient,
                TemplateName = "Mentifi_ConnectionActive"
            });
            var response = await _apiClient.ExecuteAsync<object>(apiRequest);

            if (response.IsSuccessful)
            {
                return model;
            }
            _logger.LogError(response.ErrorException, response.ErrorMessage, null);
            throw new ApplicationException(response.ErrorMessage);
        }

        public async Task<object> AddGoalProgress(EmailParam model)
        {
            _apiClient.BaseUrl = new Uri(_configuration["EmailServiceUrl"]);

            var apiRequest = new ApiRequest("", RequestMethod.POST) { RequestFormat = DataFormat.Json };
            apiRequest.AddBody(new EmailApiModel()
            {
                EmailType = "MentifiGoalProgressNotification",
                SystemUserId = model.SystemUserId,
                Payload = model.Payload,
                Recipient = model.Recipient,
                TemplateName = "Mentifi_GoalProgressNotification"
            });
            var response = await _apiClient.ExecuteAsync<object>(apiRequest);

            if (response.IsSuccessful)
            {
                return model;
            }
            _logger.LogError(response.ErrorException, response.ErrorMessage, null);
            throw new ApplicationException(response.ErrorMessage);
        }

        public async Task<object> NewMessage(EmailParam model)
        {
            _apiClient.BaseUrl = new Uri(_configuration["EmailServiceUrl"]);

            var apiRequest = new ApiRequest("", RequestMethod.POST) { RequestFormat = DataFormat.Json };
            apiRequest.AddBody(new EmailApiModel()
            {
                EmailType = "SendMessage",
                SystemUserId = model.SystemUserId,
                Payload = model.Payload,
                Recipient = model.Recipient,
                TemplateName = "mentifi_sendmessage"
            });
            var response = await _apiClient.ExecuteAsync<object>(apiRequest);

            if (response.IsSuccessful)
            {
                return model;
            }
            _logger.LogError(response.ErrorException, response.ErrorMessage, null);
            throw new ApplicationException(response.ErrorMessage);
        }

        public async Task<object> InviteAdmin(EmailParam model)
        {
            _apiClient.BaseUrl = new Uri(_configuration["EmailServiceUrl"]);

            var apiRequest = new ApiRequest("", RequestMethod.POST) { RequestFormat = DataFormat.Json };
            apiRequest.AddBody(new EmailApiModel()
            {
                EmailType = "AdminInvitation",
                SystemUserId = model.SystemUserId,
                Payload = model.Payload,
                Recipient = model.Recipient,
                TemplateName = "mentifi_admininvite"
            });
            var response = await _apiClient.ExecuteAsync<object>(apiRequest);

            if (response.IsSuccessful)
            {
                return model;
            }
            _logger.LogError(response.ErrorException, response.ErrorMessage, null);
            throw new ApplicationException(response.ErrorMessage);
        }
    }
}
