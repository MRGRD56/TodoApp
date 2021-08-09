using Newtonsoft.Json;

namespace TodoApp.DesktopClient.Extensions
{
    public static class StringExtensions
    {
        public static T ParseJson<T>(this string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }
    }
}
