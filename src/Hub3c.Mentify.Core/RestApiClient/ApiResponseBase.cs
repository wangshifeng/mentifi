using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Hub3c.Mentify.Core.RestApiClient
{
    public abstract class ApiResponseBase
    {
        private string _content;

        protected ApiResponseBase()
        {
            ResponseStatus = ResponseStatus.None;
            Headers = new List<Parameter>();
        }

        public IApiRequest Request { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public string ContentEncoding { get; set; }

        public string Content
        {
            get
            {
                if (string.IsNullOrEmpty(_content))
                    if (RawBytes != null)
                        _content = Encoding.UTF8.GetString(RawBytes, 0, RawBytes.Length);
                    else
                        _content = string.Empty;

                return _content;
            }
            set => _content = value;
        }

        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccessful => (int) StatusCode >= 200 && (int) StatusCode <= 299 &&
                                    ResponseStatus == ResponseStatus.Completed;

        public string StatusDescription { get; set; }
        public byte[] RawBytes { get; set; }
        public string Server { get; set; }

        public IList<Parameter> Headers { get; protected internal set; }

        public ResponseStatus ResponseStatus { get; set; }
        public string ErrorMessage { get; set; }
        public Exception ErrorException { get; set; }
        public Version ProtocolVersion { get; set; }
    }
}