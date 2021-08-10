using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.DesktopClient.Services
{
    public static class EmbeddedResources
    {
        private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();

        public static string[] GetNames() => ExecutingAssembly.GetManifestResourceNames();

        public static Stream GetStream(string resourceName) =>
            ExecutingAssembly.GetManifestResourceStream(resourceName);
    }
}
