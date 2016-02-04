using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Yakka.Util
{
    class FlashWindow : INotifier
    {
        private IntPtr _mainWindowHWnd;
        //private Window _window;
        private Application _app;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            /// <summary>
            /// The size of the structure in bytes.
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
            /// </summary>
            public IntPtr hwnd;
            /// <summary>
            /// The Flash Status.
            /// </summary>
            public uint dwFlags;
            /// <summary>
            /// The number of times to Flash the window.
            /// </summary>
            public uint uCount;
            /// <summary>
            /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
            /// </summary>
            public uint dwTimeout;
        }

        /// <summary>
        /// Stop flashing. The system restores the window to its original stae.
        /// </summary>
        private const uint FLASHW_STOP = 0;

        /// <summary>
        /// Flash the window caption.
        /// </summary>
        private const uint FLASHW_CAPTION = 1;

        /// <summary>
        /// Flash the taskbar button.
        /// </summary>
        private const uint FLASHW_TRAY = 2;

        /// <summary>
        /// Flash both the window caption and taskbar button.
        /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
        /// </summary>
        private const uint FLASHW_ALL = 3;

        /// <summary>
        /// Flash continuously, until the FLASHW_STOP flag is set.
        /// </summary>
        private const uint FLASHW_TIMER = 4;

        /// <summary>
        /// Flash continuously until the window comes to the foreground.
        /// </summary>
        private const uint FLASHW_TIMERNOFG = 12;

        public FlashWindow()
        {
            var cur = Application.Current;
            _app = cur;
        }

        public void NotifyUser()
        {
            //todo: Find a decent way of getting a handle... this doesn't work
            if (!_app.MainWindow.IsFocused)
                FlashApplicationWindow();
        }

        private void FlashApplicationWindow()
        {
            InitializeHandle();
            Flash(this._mainWindowHWnd, 5);
        }

        /// <summary>
        /// Flash the spacified Window (Form) until it recieves focus.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        private static bool Flash(IntPtr hwnd)
        {
            // Make sure we're running under Windows 2000 or later
            if (Win2000OrLater)
            {
                FLASHWINFO fi = CreateFlashInfoStruct(hwnd, FLASHW_ALL | FLASHW_TIMERNOFG, uint.MaxValue, 0);

                return FlashWindowEx(ref fi);
            }
            return false;
        }

        private static bool Flash(IntPtr hwnd, uint count)
        {
            if (Win2000OrLater)
            {
                FLASHWINFO fi = CreateFlashInfoStruct(hwnd, FLASHW_ALL | FLASHW_TIMERNOFG, count, 0);

                return FlashWindowEx(ref fi);
            }

            return false;
        }

        private static FLASHWINFO CreateFlashInfoStruct(IntPtr handle, uint flags, uint count, uint timeout)
        {
            FLASHWINFO fi = new FLASHWINFO();
            fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
            fi.hwnd = handle;
            fi.dwFlags = flags;
            fi.uCount = count;
            fi.dwTimeout = timeout;
            return fi;
        }

        private void StopFlashing()
        {
            InitializeHandle();

            if (Win2000OrLater)
            {
                FLASHWINFO fi = CreateFlashInfoStruct(this._mainWindowHWnd, FLASHW_STOP, uint.MaxValue, 0);
                FlashWindowEx(ref fi);
            }
        }

        private void InitializeHandle()
        {
            if (this._mainWindowHWnd == IntPtr.Zero)
            {
                // Delayed creation of Main Window IntPtr as Application.Current passed in to ctor does not have the MainWindow set at that time
                var mainWindow = _app.MainWindow;
                this._mainWindowHWnd = new System.Windows.Interop.WindowInteropHelper(mainWindow).Handle;
            }
        }

        private static bool Win2000OrLater
        {
            get { return Environment.OSVersion.Version.Major >= 5; }
        }
    }
}
