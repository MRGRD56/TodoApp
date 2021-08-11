using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TodoApp.DesktopClient.Extensions
{
    public static class Validation
    {
        public static async Task<string> ParseServerErrorsStringAsync(HttpContent httpContent) =>
            string.Join("\n", await ParseServerErrorsAsync(httpContent));

        public static async Task<string[]> ParseServerErrorsAsync(HttpContent httpContent)
        {
            var errors = new List<string>();

            var mediaType = httpContent.Headers.ContentType?.MediaType;
            var isJson = false;
            if (mediaType != null)
            {
                isJson = Regex.IsMatch(mediaType, @"^\s*application\/.*json\s*$");
            }

            if (isJson)
            {
                var response = await httpContent.ParseJsonAsync();
                if (response["errors"]?.HasValues == true)
                {
                    var errorsDictionary = response["errors"].ToString().ParseJson<Dictionary<string, string[]>>();
                    if (errorsDictionary != null)
                    {
                        var errorsStrings = errorsDictionary
                            .Select(kv => kv.Value)
                            .Aggregate((a, b) => a.Concat(b).ToArray());
                        errors.AddRange(errorsStrings);
                    }
                }
            }
            else
            {
                var stringResponse = await httpContent.ReadAsStringAsync();
                errors.Add(stringResponse);
            }

            return errors.ToArray();
        }
    }
}
