using System;
using System.Net.Http;

namespace Hub3c.Mentify.Core.RestApiClient
{
    public interface IHttpClientFactory : IDisposable
    {
        HttpClient Create(Uri baseAddress);
        HttpClient Create(string baseAddress);
        HttpClient Create();
    }
}