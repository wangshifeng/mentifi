using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;

namespace Hub3c.Mentify.Core.RestApiClient
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly ConcurrentDictionary<Uri, HttpClient> _httpClients;
        private bool _disposed;

        public HttpClientFactory()
        {
            _httpClients = new ConcurrentDictionary<Uri, HttpClient>();
        }

        public HttpClient Create(Uri baseAddress)
        {
            if (baseAddress == null) throw new ArgumentNullException(nameof(baseAddress));

            return _httpClients.GetOrAdd(baseAddress, b => new HttpClient {BaseAddress = b});
        }

        public HttpClient Create(string baseAddress)
        {
            if (string.IsNullOrEmpty(baseAddress) || !Uri.IsWellFormedUriString(baseAddress, UriKind.Absolute))
                throw new ArgumentException("Invalid base address format", nameof(baseAddress));

            return Create(new Uri(baseAddress, UriKind.Absolute));
        }

        public HttpClient Create()
        {
            return Create(new Uri("/", UriKind.Relative));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
                if (_httpClients.Any())
                {
                    foreach (var httpClient in _httpClients.Values) httpClient.Dispose();
                    _httpClients.Clear();
                }

            _disposed = true;
        }
    }
}