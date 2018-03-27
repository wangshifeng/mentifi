using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Hub3c.Mentify.Core.Extensions;
using Hub3c.Mentify.Core.RestApiClient.Authenticators;
using Hub3c.Mentify.Core.Serialization.Deserializers;

namespace Hub3c.Mentify.Core.RestApiClient
{
    public class ApiClient : IApiClient
    {
        private static readonly Version Version = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version;
        private static readonly Regex StructuredSyntaxSuffixRegex = new Regex(@"\+\w+$", RegexOptions.Compiled);

        private static readonly Regex StructuredSyntaxSuffixWildcardRegex =
            new Regex(@"^\*\+\w+$", RegexOptions.Compiled);

        private static readonly IHttpClientFactory HttpClientFactory = new HttpClientFactory();

        public ApiClient()
        {
            Encoding = Encoding.UTF8;
            ContentHandlers = new Dictionary<string, IDeserializer>();
            AcceptTypes = new List<string>();
            DefaultParameters = new List<Parameter>();

            AddHandler("application/json", new JsonDeserializer());
            AddHandler("application/xml", new XmlDeserializer());
            AddHandler("application/x-www-form-urlencoded", new FormUrlEncodedDeserializer());
            AddHandler("text/json", new JsonDeserializer());
            AddHandler("text/x-json", new JsonDeserializer());
            AddHandler("text/javascript", new JsonDeserializer());
            AddHandler("text/xml", new XmlDeserializer());
            AddHandler("*+json", new JsonDeserializer());
            AddHandler("*+xml", new XmlDeserializer());
            AddHandler("*", new XmlDeserializer());
        }

        public ApiClient(Uri baseUrl) : this()
        {
            BaseUrl = baseUrl;
        }

        public ApiClient(string baseUrl) : this()
        {
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));

            BaseUrl = new Uri(baseUrl);
        }

        private IDictionary<string, IDeserializer> ContentHandlers { get; }
        private IList<string> AcceptTypes { get; }
        public string UserAgent { get; set; }
        public int Timeout { get; set; }
        public IAuthenticator Authenticator { get; set; }
        public Uri BaseUrl { get; set; }
        public Encoding Encoding { get; set; }
        public IList<Parameter> DefaultParameters { get; }

        public IApiResponse<T> Deserialize<T>(IApiResponse response)
        {
            IApiResponse<T> result = new ApiResponse<T>();

            try
            {
                result = response.ToGenericResponse<T>();
                result.Request = response.Request;
                if (result.ErrorException == null && result.ContentType != null)
                {
                    var handler = GetHandler(response.ContentType);
                    if (handler != null)
                        result.Data = handler.Deserialize<T>(response.Content);
                }
            }
            catch (Exception ex)
            {
                result.ResponseStatus = ResponseStatus.Error;
                result.ErrorMessage = ex.Message;
                result.ErrorException = ex;
            }

            return result;
        }

        public async Task<IApiResponse> ExecuteAsync(IApiRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Authenticator?.Authenticate(request);

            var response = new ApiResponse();

            try
            {
                var httpClient = BaseUrl != null
                    ? HttpClientFactory.Create(BaseUrl)
                    : HttpClientFactory.Create();

                var userAgent = UserAgent ?? $"ApiClient/{Version}";
                httpClient.DefaultRequestHeaders.UserAgent.Clear();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                var timeout = request.Timeout > 0 ? request.Timeout : Timeout;
                if (timeout > 0) httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);

                //ConfigureHttpClient(httpClient);
                foreach (var parameter in DefaultParameters)
                {
                    if (request.Parameters.Any(p => p.Name == parameter.Name && p.Type == parameter.Type)) continue;
                    request.Parameters.Add(parameter);
                }

                if (request.Parameters.All(p => p.Name.ToLowerInvariant() != "accept"))
                {
                    var accepts = string.Join(", ", AcceptTypes);
                    request.AddParameter("Accept", accepts, ParameterType.HttpHeader);
                }

                var requestMethod = GetRequestMethod(request.Method);
                var requestUrl = BuildUri(request);
                var httpRequest = new HttpRequestMessage(requestMethod, requestUrl);

                var headers = request.Parameters
                    .Where(p => p.Type == ParameterType.HttpHeader)
                    .Select(p => new { p.Name, Value = Convert.ToString(p.Value) });

                foreach (var header in headers)
                {
                    if (header.Name.Equals("Authorization"))
                    {
                        if (header.Value.Contains("key="))
                            httpRequest.Headers.TryAddWithoutValidation(header.Name, header.Value);
                        else
                            httpRequest.Headers.Add(header.Name, header.Value);
                    }
                    else
                        httpRequest.Headers.Add(header.Name, header.Value);
                }

                var parameters = request.Parameters
                    .Where(p => p.Type == ParameterType.GetOrPost && p.Value != null)
                    .Select(p => new { p.Name, p.Value });
                foreach (var parameter in parameters)
                    httpRequest.Properties.Add(parameter.Name, parameter.Value);

                var body = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
                if (body != null)
                    httpRequest.Content = new StringContent(Convert.ToString(body.Value), Encoding, body.Name);

                var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken);

                response = await ConvertToApiResponse(request, httpResponse);
                response.Request = request;
                response.Request.IncreaseNumAttempts();
            }
            catch (Exception ex)
            {
                response.ResponseStatus = ResponseStatus.Error;
                response.ErrorMessage = ex.Message;
                response.ErrorException = ex;
            }

            return response;
        }

        public async Task<IApiResponse<T>> ExecuteAsync<T>(IApiRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Deserialize<T>(await ExecuteAsync(request, cancellationToken));
        }

        public Uri BuildUri(IApiRequest request)
        {
            if (BaseUrl == null) throw new NullReferenceException("ApiClient must contain a value for BaseUrl");

            var assembled = request.Resource;
            var urlParameters = request.Parameters.Where(p => p.Type == ParameterType.UrlSegment);
            var builder = new UriBuilder(BaseUrl);

            foreach (var parameter in urlParameters)
            {
                if (parameter.Value == null)
                    throw new ArgumentException(
                        $"Cannot build uri when url segment parameter '{parameter.Name}' value is null.",
                        nameof(request));

                if (!string.IsNullOrEmpty(assembled))
                    assembled = assembled.Replace("{" + parameter.Name + "}", parameter.Value.ToString().UrlEncode());

                builder.Path = builder.Path.UrlDecode()
                    .Replace("{" + parameter.Name + "}", parameter.Value.ToString().UrlEncode());
            }

            BaseUrl = new Uri(builder.ToString());

            if (!string.IsNullOrEmpty(assembled) && assembled.StartsWith("/"))
                assembled = assembled.Substring(1);

            if (!string.IsNullOrEmpty(BaseUrl.AbsoluteUri))
            {
                if (!BaseUrl.AbsoluteUri.EndsWith("/") && !string.IsNullOrEmpty(assembled))
                    assembled = string.Concat("/", assembled);

                assembled = string.IsNullOrEmpty(assembled)
                    ? BaseUrl.AbsoluteUri
                    : string.Format("{0}{1}", BaseUrl, assembled);
            }

            IEnumerable<Parameter> parameters;
            if (request.Method != RequestMethod.POST && request.Method != RequestMethod.PUT)
                parameters = request.Parameters
                    .Where(p => p.Type == ParameterType.GetOrPost || p.Type == ParameterType.QueryString)
                    .ToList();
            else
                parameters = request.Parameters
                    .Where(p => p.Type == ParameterType.QueryString)
                    .ToList();

            if (!parameters.Any()) return new Uri(assembled);

            var data = string.Join("&", parameters.Select(EncodeParameter));
            var separator = assembled != null && assembled.Contains("?") ? "&" : "?";

            assembled = string.Concat(assembled, separator, data);
            return new Uri(assembled);
        }

        public void AddHandler(string contentType, IDeserializer deserializer)
        {
            ContentHandlers[contentType] = deserializer;

            if (contentType == "*" || StructuredSyntaxSuffixWildcardRegex.IsMatch(contentType)) return;

            AcceptTypes.Add(contentType);
            var accepts = string.Join(", ", AcceptTypes);
            RemoveDefaultParameter("Accept");
            AddDefaultParameter("Accept", accepts, ParameterType.HttpHeader);
        }

        public void RemoveHandler(string contentType)
        {
            ContentHandlers.Remove(contentType);
            AcceptTypes.Remove(contentType);
            RemoveDefaultParameter("Accept");
        }

        public void ClearHandlers()
        {
            ContentHandlers.Clear();
            AcceptTypes.Clear();
            RemoveDefaultParameter("Accept");
        }

        public void RemoveDefaultParameter(string parameterName)
        {
            var parameter = DefaultParameters
                .SingleOrDefault(p => p.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
            if (parameter == null) return;

            DefaultParameters.Remove(parameter);
        }

        public void AddDefaultParameter(Parameter parameter)
        {
            if (parameter.Type == ParameterType.RequestBody)
                throw new NotSupportedException(
                    "Cannot set request body from default headers. Use Request.AddBody() instead.");

            DefaultParameters.Add(parameter);
        }

        public void AddDefaultParameter(string parameterName, object value, ParameterType parameterType)
        {
            AddDefaultParameter(new Parameter { Name = parameterName, Value = value, Type = parameterType });
        }

        private string EncodeParameter(Parameter parameter)
        {
            return parameter.Value == null
                ? string.Concat(parameter.Name.UrlEncode(), "=")
                : string.Concat(parameter.Name.UrlEncode(), "=", parameter.Value.ToString().UrlEncode());
        }

        private IDeserializer GetHandler(string contentType)
        {
            if (contentType == null) throw new ArgumentNullException(nameof(contentType));

            if (string.IsNullOrEmpty(contentType) && ContentHandlers.ContainsKey("*"))
                return ContentHandlers["*"];

            var semicolonIndex = contentType.IndexOf(';');
            if (semicolonIndex > -1) contentType = contentType.Substring(0, semicolonIndex);

            if (ContentHandlers.ContainsKey(contentType))
                return ContentHandlers[contentType];

            var structuredSyntaxSuffixMatch = StructuredSyntaxSuffixRegex.Match(contentType);
            if (!structuredSyntaxSuffixMatch.Success)
                return ContentHandlers.ContainsKey("*") ? ContentHandlers["*"] : null;

            var structuredSyntaxSuffixWildcard = "*" + structuredSyntaxSuffixMatch.Value;
            if (ContentHandlers.ContainsKey(structuredSyntaxSuffixWildcard))
                return ContentHandlers[structuredSyntaxSuffixWildcard];

            return ContentHandlers.ContainsKey("*") ? ContentHandlers["*"] : null;
        }

        private async Task<ApiResponse> ConvertToApiResponse(IApiRequest request, HttpResponseMessage httpResponse)
        {
            var response = new ApiResponse
            {
                Request = request,
                Content = await httpResponse.Content.ReadAsStringAsync(),
                ContentEncoding = httpResponse.Content.Headers.ContentEncoding.FirstOrDefault(),
                ContentLength = httpResponse.Content.Headers.ContentLength ?? 0,
                ContentType = httpResponse.Content.Headers?.ContentType?.MediaType,
                ErrorException = null,
                ErrorMessage = null,
                RawBytes = await httpResponse.Content.ReadAsByteArrayAsync(),
                ResponseStatus =
                    httpResponse.IsSuccessStatusCode
                        ? ResponseStatus.Completed
                        : ResponseStatus.Error, /// TODO: Maps proper response status from status code
                ProtocolVersion = httpResponse.Version,
                Server = httpResponse.Headers.Server.ToString(),
                StatusCode = httpResponse.StatusCode,
                StatusDescription = httpResponse.ReasonPhrase
            };

            foreach (var header in httpResponse.Headers)
                response.Headers.Add(new Parameter
                {
                    Name = header.Key,
                    Value = string.Join(" ", header.Value),
                    Type = ParameterType.HttpHeader
                });

            return response;
        }

        private HttpMethod GetRequestMethod(RequestMethod method)
        {
            switch (method)
            {
                case RequestMethod.GET: return HttpMethod.Get;
                case RequestMethod.POST: return HttpMethod.Post;
                case RequestMethod.PUT: return HttpMethod.Put;
                case RequestMethod.DELETE: return HttpMethod.Delete;
                case RequestMethod.HEAD: return HttpMethod.Head;
                case RequestMethod.OPTIONS: return HttpMethod.Options;
                default: return HttpMethod.Get;
            }
        }
    }
}