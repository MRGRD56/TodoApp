using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoApp.DesktopClient.Extensions
{
    public static class HttpClientExtensions
    {
        private static HttpContent CreateHttpContent(object source)
        {
            return new StringContent(JsonConvert.SerializeObject(source));
        }

        public static async Task<TResponse> GetAsync<TResponse>(this HttpClient httpClient, string requestUri)
        {
            return await (await httpClient.GetAsync(requestUri)).Content.ParseJsonAsync<TResponse>();
        }

        public static async Task<TResponse> PostAsync<TResponse>(this HttpClient httpClient, string requestUri, object body)
        {
            return await (await httpClient.PostAsync(requestUri, CreateHttpContent(body))).Content.ParseJsonAsync<TResponse>();
        }

        public static async Task<TResponse> PutAsync<TResponse>(this HttpClient httpClient, string requestUri, object body)
        {
            return await (await httpClient.PutAsync(requestUri, CreateHttpContent(body))).Content.ParseJsonAsync<TResponse>();
        }

        public static async Task<TResponse> DeleteAsync<TResponse>(this HttpClient httpClient, string requestUri, object body)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(requestUri),
                Method = HttpMethod.Delete,
                Content = CreateHttpContent(body)
            };
            return await (await httpClient.SendAsync(request)).Content.ParseJsonAsync<TResponse>();
        }
    }
}
