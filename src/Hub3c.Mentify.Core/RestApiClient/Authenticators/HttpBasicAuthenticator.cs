using System;
using System.Linq;
using System.Text;

namespace Hub3c.Mentify.Core.RestApiClient.Authenticators
{
    public class HttpBasicAuthenticator : IAuthenticator
    {
        private readonly string _authHeader;

        public HttpBasicAuthenticator(string username, string password)
        {
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password)));
            _authHeader = $"Basic {token}";
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