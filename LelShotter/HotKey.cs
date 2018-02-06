using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace LelShotter
{
    public class HotKey : IDisposable
    {
        private static Dictionary<int, HotKey> _dictHotKeyToCallBackProc;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const int WmHotKey = 0x0312;

        private bool _disposed;

        public Key Key { get; }
        public KeyModifier KeyModifiers { get; }
        public Action<HotKey, DataModels.ScreenshotMode> Action { get; }
        public int Id { get; set; }

        public HotKey(Key k, KeyModifier keyModifiers, Action<HotKey, DataModels.ScreenshotMode> action, bool register = true)
        {
            Key = k;
            KeyModifiers = keyModifiers;
            Action = action;
            if (register)
            {
                Register();
            }
        }
        
        public bool Register()
        {
            var virtualKeyCode = KeyInterop.VirtualKeyFromKey(Key);
            Id = virtualKeyCode + (int)KeyModifiers * 0x10000;
            var result = RegisterHotKey(IntPtr.Zero, Id, (uint)KeyModifiers, (uint)virtualKeyCode);

            if (_dictHotKeyToCallBackProc == null)
            {
                _dictHotKeyToCallBackProc = new Dictionary<int, HotKey>();
                ComponentDispatcher.ThreadFilterMessage += ComponentDispatcherThreadFilterMessage;
            }

            _dictHotKeyToCallBackProc.Add(Id, this);
            
            return result;
        }
        
        public void Unregister()
        {
            if (_dictHotKeyToCallBackProc.TryGetValue(Id, out HotKey _))
            {
                UnregisterHotKey(IntPtr.Zero, Id);
            }
        }
        
        private static void ComponentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (handled)
            {
                return;
            }
            if (msg.message != WmHotKey)
            {
                return;
            }

            if (!_dictHotKeyToCallBackProc.TryGetValue((int) msg.wParam, out HotKey hotKey))
            {
                return;
            }
            hotKey.Action?.Invoke(hotKey, DataModels.ScreenshotMode.FullScreen); // todo check how to pass one more arg
            handled = true;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                Unregister();
            }
                
            _disposed = true;
        }
    }
    
    [Flags]
    public enum KeyModifier
    {
        None = 0x0000,
        Alt = 0x0001,
        Ctrl = 0x0002,
        NoRepeat = 0x4000,
        Shift = 0x0004,
        Win = 0x0008
    }
}
