using System;
using System.Collections.Generic;
using System.Net;

namespace Hub3c.Mentify.Core.RestApiClient
{
    public interface IApiResponse
    {
        IApiRequest Request { get; set; }
        string ContentType { get; set; }
        long ContentLength { get; set; }
        string ContentEncoding { get; set; }
        string Content { get; set; }
        HttpStatusCode StatusCode { get; set; }
        bool IsSuccessful { get; }
        string StatusDescription { get; set; }
        byte[] RawBytes { get; set; }
        string Server { get; set; }
        IList<Parameter> Headers { get; }
        ResponseStatus ResponseStatus { get; set; }
        string ErrorMessage { get; set; }
        Exception ErrorException { get; set; }
        Version ProtocolVersion { get; set; }
    }

    public interface IApiResponse<T> : IApiResponse
    {
        T Data { get; set; }
    }

    public static class ApiResponseExtensions
    {
        public static IApiResponse<T> ToGenericResponse<T>(this IApiResponse response)
        {
            return new ApiResponse<T>
            {
                ContentEncoding = response.ContentEncoding,
                ContentLength = response.ContentLength,
                ContentType = response.ContentType,
                ErrorException = response.ErrorException,
                ErrorMessage = response.ErrorMessage,
                Headers = response.Headers,
                RawBytes = response.RawBytes,
                ResponseStatus = response.ResponseStatus,
                Server = response.Server,
                StatusCode = response.StatusCode,
                StatusDescription = response.StatusDescription
            };
        }
    }
}