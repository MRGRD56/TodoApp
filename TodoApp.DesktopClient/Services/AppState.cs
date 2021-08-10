using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.DesktopClient.Services
{
    public static class AppState
    {
        private static bool _isLoading = true;

        public static bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                IsLoadingChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static event EventHandler IsLoadingChanged;
    }
}
