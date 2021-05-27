using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace RimGen.Lib
{
    public class Game
    {
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        }

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hdc, uint nFlags);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr handle, ref Rectangle rect);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        private static IntPtr gameWindowHandle;

        public static void Init()
        {
            GetWindowByName("RimWorld by Ludeon Studios");
            ShowWindow(gameWindowHandle, ShowWindowEnum.Restore);
        }

        public static void SendGenerateNew()
        {
            //ShowWindow(gameWindowHandle, ShowWindowEnum.Restore);
            Clicker.ClickOnPoint(gameWindowHandle, new Point(1357, 255));
        }

        public static void SaveScreenshot()
        {
            var bmp = GetScreenshot();
            string screenshotPath = "rimworld.bmp";
            bmp.Save(screenshotPath, ImageFormat.Bmp);
            bmp.Dispose();
        }

        public static Bitmap GetScreenshot()
        {
            var rect = new Rectangle();
            GetWindowRect(gameWindowHandle, ref rect);

            var bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            }

            return bmp;
        }

        private static void GetWindowByName(string wName)
        {
            IntPtr hWnd = IntPtr.Zero;
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains(wName))
                {
                    hWnd = pList.MainWindowHandle;
                }
            }
            gameWindowHandle = hWnd;

            if (hWnd == IntPtr.Zero) {
                throw new Exception(Form1.Lang == "ru" ? "Ошибка! Окно с игрой не найдено" : "Error! Game window not found");
            }
        }
    }
}
