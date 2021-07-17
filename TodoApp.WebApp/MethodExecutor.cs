using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TodoApp.WebApp
{
    [Obsolete("Do not use", true)]
    public static class MethodExecutor
    {
        private static IActionResult GetResultFromException(Exception exception)
        {
            if (exception is HttpRequestException httpRequestException)
            {
                return new ObjectResult(httpRequestException.Message)
                {
                    StatusCode = (int) (httpRequestException.StatusCode ?? HttpStatusCode.InternalServerError)
                };
            }

            throw exception;
        }
        
        public static IActionResult GetActionResult<TResult>(Func<TResult> action)
        {
            try
            {
                var result = action();
                return new ObjectResult(result);
            }
            catch (HttpRequestException exception)
            {
                return GetResultFromException(exception);
            }
        }
        
        public static async Task<IActionResult> GetActionResultAsync<TResult>(Func<Task<TResult>> action)
        {
            try
            {
                var result = await action();
                return new ObjectResult(result);
            }
            catch (HttpRequestException exception)
            {
                return GetResultFromException(exception);
            }
        }
    }
}