using System;
using System.Linq;

namespace Hub3c.Mentify.Core.RestApiClient.Authenticators
{
    public class HttpBearerAuthenticator : IAuthenticator
    {
        private readonly string _authHeader;

        public HttpBearerAuthenticator(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException(nameof(accessToken));

            _authHeader = $"Bearer {accessToken}";
        }

        public void Authenticate(IApiRequest request)
        {
            if (!request.Parameters.Any(p =>
                p.Type.Equals(ParameterType.HttpHeader) &&
                p.Name.Equals("Authorization", StringComparison.OrdinalIgnoreCase)))
                request.AddParameter("Authorization", _authHeader, ParameterType.HttpHeader);
        }
    }
}