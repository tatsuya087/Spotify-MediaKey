using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpotifyMediaKey
{
    public static class MediaKeyHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_VOLUME_MUTE = 0xAD;
        private const int VK_VOLUME_DOWN = 0xAE;
        private const int VK_VOLUME_UP = 0xAF;
        private const int VK_MEDIA_NEXT_TRACK = 0xB0;
        private const int VK_MEDIA_PREV_TRACK = 0xB1;
        private const int VK_MEDIA_PLAY_PAUSE = 0xB3;

        private static readonly HashSet<int> _keysCurrentlyDown = new();
        private static readonly HashSet<int> _noRepeatKeys = new()
        {
            VK_VOLUME_MUTE, VK_MEDIA_NEXT_TRACK, VK_MEDIA_PREV_TRACK, VK_MEDIA_PLAY_PAUSE
        };

        public static event Action<VolumeKeyType>? KeyPressed;

        private static IntPtr _hookId = IntPtr.Zero;
        private static LowLevelKeyboardProc _proc = HookCallback;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public static void Start()
        {
            using var curProcess = System.Diagnostics.Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule!;
            _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    VolumeKeyType? key = vkCode switch
                    {
                        VK_VOLUME_UP => VolumeKeyType.Up,
                        VK_VOLUME_DOWN => VolumeKeyType.Down,
                        VK_VOLUME_MUTE => VolumeKeyType.Mute,
                        VK_MEDIA_NEXT_TRACK => VolumeKeyType.Next,
                        VK_MEDIA_PREV_TRACK => VolumeKeyType.Previous,
                        VK_MEDIA_PLAY_PAUSE => VolumeKeyType.PlayPause,
                        _ => null
                    };

                    if (key.HasValue)
                    {
                        bool isRepeat = _keysCurrentlyDown.Contains(vkCode);
                        _keysCurrentlyDown.Add(vkCode);

                        if (!(isRepeat && _noRepeatKeys.Contains(vkCode)))
                        {
                            KeyPressed?.Invoke(key.Value);
                        }

                        return (IntPtr)1;
                    }
                }
                else if (wParam == (IntPtr)WM_KEYUP)
                {
                    _keysCurrentlyDown.Remove(vkCode);
                }
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
        public static void Stop()
        {
            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }
    }
    public enum VolumeKeyType { Up, Down, Mute, PlayPause, Next, Previous }
}