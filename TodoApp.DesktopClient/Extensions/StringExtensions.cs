using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TodoApp.DesktopClient.Extensions
{
    public static class StringExtensions
    {
        public static T ParseJson<T>(this string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }

        public static JObject ParseJson(this string source)
        {
            return source.ParseJson<JObject>();
        }
    }
}
