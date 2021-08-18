using System;
using System.IO;
using TodoApp.MobileClient.Droid.Models;
using TodoApp.MobileClient.Models;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidLocalDbFileDirectoryProvider))]
namespace TodoApp.MobileClient.Droid.Models
{
    public class AndroidLocalDbFileDirectoryProvider : ILocalDbFileDirectoryProvider
    {
        public string GetLocalDbFileDirectory() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
    }
}