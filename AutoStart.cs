using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace SpotifyMediaKey
{
    public static class AutoStart
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "SpotifyMediaKey";

        public static bool IsAutoStartEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false);
            return key?.GetValue(AppName) != null;
        }

        public static void SetAutoStart(bool enable)
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);
            if (key == null) return;

            if (enable)
            {
                string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName;
                key.SetValue(AppName, exePath);
            }
            else
            {
                key.DeleteValue(AppName, false);
            }
        }
    }
}