using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TodoApp.DesktopClient.Extensions;

namespace TodoApp.DesktopClient.Models.Exceptions
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
