namespace Hub3c.Mentify.Core.RestApiClient.Authenticators
{
    public interface IAuthenticator
    {
        void Authenticate(IApiRequest request);
    }
}