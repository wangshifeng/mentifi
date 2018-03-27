namespace Hub3c.Mentify.Core.RestApiClient
{
    public class ApiResponse<T> : ApiResponseBase, IApiResponse<T>
    {
        public T Data { get; set; }

        public static explicit operator ApiResponse<T>(ApiResponse response)
        {
            return new ApiResponse<T>
            {
                ContentEncoding = response.ContentEncoding,
                ContentLength = response.ContentLength,
                ContentType = response.ContentType,
                ErrorMessage = response.ErrorMessage,
                ErrorException = response.ErrorException,
                Headers = response.Headers,
                RawBytes = response.RawBytes,
                ResponseStatus = response.ResponseStatus,
                ProtocolVersion = response.ProtocolVersion,
                Server = response.Server,
                StatusCode = response.StatusCode,
                StatusDescription = response.StatusDescription,
                Request = response.Request
            };
        }
    }

    public class ApiResponse : ApiResponseBase, IApiResponse
    {
    }
}