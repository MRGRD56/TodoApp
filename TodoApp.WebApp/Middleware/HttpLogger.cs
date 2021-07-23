using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TodoApp.WebApp.Middleware
{
    public class HttpLogger
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly Regex _requestUrlPattern;

        public HttpLogger(RequestDelegate next, ILoggerFactory loggerFactory, Regex requestUrlPattern = null)
        {
            _next = next;
            _requestUrlPattern = requestUrlPattern;
            _logger = loggerFactory.CreateLogger<HttpLogger>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            finally
            {
                if (httpContext != null &&
                    _requestUrlPattern?.IsMatch(httpContext.Request.Path.Value ?? string.Empty) != false)
                {
//                     var logContent = $@"
// ┌─ Request
// │  {httpContext.Request.Method.ToUpper()} {httpContext.Request.Path.Value}
// │  {(string.IsNullOrEmpty(requestBody) ? "" : $"[Body]\n│  {requestBody.Replace("\n", "\n│  ")}")}
// ├─ Response
// │  {httpContext.Response.StatusCode} {(HttpStatusCode) httpContext.Response.StatusCode}";

                    var requestPath = httpContext.Request.Path.Value + httpContext.Request.QueryString.Value;
                    
                    var logContent = 
                        $"┌─ {httpContext.Request.Method.ToUpper()} {requestPath}\n" + 
                        $"└─ {httpContext.Response.StatusCode} {(HttpStatusCode) httpContext.Response.StatusCode}";
                    _logger.LogInformation(logContent);
                }
            }
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            var bodyContent = await reader.ReadToEndAsync();

            //request.Body.Position = 0;

            return bodyContent;
        }
    }
}