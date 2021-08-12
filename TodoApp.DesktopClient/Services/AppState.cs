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
        private static bool _isConnecting;

        public static bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                IsLoadingChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static bool IsConnecting
        {
            get => _isConnecting;
            set
            {
                _isConnecting = value;
                IsConnectingChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static event EventHandler IsLoadingChanged;
        public static event EventHandler IsConnectingChanged;
    }
}
