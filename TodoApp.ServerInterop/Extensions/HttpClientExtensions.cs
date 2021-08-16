using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using TodoApp.ServerInterop.Models.Exceptions;

namespace TodoApp.ServerInterop.Extensions
{
    public static class HttpClientExtensions
    {
        private static HttpContent CreateHttpContent(object source)
        {
            var json = JsonConvert.SerializeObject(source);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public static async Task<TResponse> SendAsync<TResponse>(
            this HttpClient httpClient, string requestUri,
            HttpMethod method, object body = null)
        {
            //try
            //{
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(requestUri),
                Method = method,
                Content = body == null ? null : CreateHttpContent(body)
            };
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ParseJsonAsync<TResponse>();
            }

            throw new HttpException(response);
            //}
            //catch (Exception exception)
            //{
            //    //if (exception is HttpRequestException or TaskCanceledException)
            //    //{
            //    //    MessageBox.Show("Failed to connect to the server. Check your internet connection.", 
            //    //        "Connection error", 
            //    //        MessageBoxButton.OK, MessageBoxImage.Error);
            //    //}
            //    //TODO
            //    throw;
            //}
        }

        public static async Task<TResponse> GetAsync<TResponse>(this HttpClient httpClient, string requestUri, object body = null)
        {
            return await httpClient.SendAsync<TResponse>(requestUri, HttpMethod.Get, body);
        }

        public static async Task<TResponse> PostAsync<TResponse>(this HttpClient httpClient, string requestUri, object body = null)
        {
            return await httpClient.SendAsync<TResponse>(requestUri, HttpMethod.Post, body);
        }

        public static async Task<TResponse> PutAsync<TResponse>(this HttpClient httpClient, string requestUri, object body = null)
        {
            return await httpClient.SendAsync<TResponse>(requestUri, HttpMethod.Put, body);
        }

        public static async Task<TResponse> DeleteAsync<TResponse>(this HttpClient httpClient, string requestUri, object body = null)
        {
            return await httpClient.SendAsync<TResponse>(requestUri, HttpMethod.Delete, body);
        }
    }
}
