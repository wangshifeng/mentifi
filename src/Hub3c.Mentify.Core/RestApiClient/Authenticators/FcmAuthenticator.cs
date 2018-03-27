using System;
using System.Linq;
using System.Text;

namespace Hub3c.Mentify.Core.RestApiClient.Authenticators
{
    public class FcmAuthenticator : IAuthenticator
    {
        private readonly string _authHeader;

        public FcmAuthenticator(string token)
        {
            _authHeader = $"key={token}";
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