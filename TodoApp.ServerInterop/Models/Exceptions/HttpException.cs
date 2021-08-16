using System;
using System.Net.Http;

namespace TodoApp.ServerInterop.Models.Exceptions
{
    public class HttpException : Exception
    {
        public HttpResponseMessage Response { get; }

        public HttpException(HttpResponseMessage response)
        {
            Response = response;
        }
    }
}
