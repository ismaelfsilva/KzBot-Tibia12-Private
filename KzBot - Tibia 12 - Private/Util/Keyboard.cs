using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KzBot
{
    public static class Keyboard
    {
        public static void PressChar(uint key)
        {
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_CHAR, key, 0);
        }

        public static void Write(string text)
        {
            foreach (byte b in ASCIIEncoding.Default.GetBytes(text))
            {
                PressChar(b);
            }
        }

        public static uint getlParam(uint key, bool up = false, bool extended = false)
        {
            uint scanCode = WinApi.MapVirtualKey(key, 0);
            uint lParam = (0x00000001 | (scanCode << 16));
            if (extended)
                lParam |= 0x01000000;

            if (up)
                return (lParam | 0xC0000000);

            return lParam;
        }

        public static void PressKey(uint key)
        {
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_KEYDOWN, key, getlParam(key, false, true));
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_KEYUP, key, getlParam(key, true, true));
        }

        public static void PressKey(Keys key, int delay = 0)
        {
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_KEYDOWN, (uint)key, getlParam((uint)key, false, true));
            if (delay > 0)
                System.Threading.Thread.Sleep(delay);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_KEYUP, (uint)key, getlParam((uint)key, true, true));
        }
        //

        public static void PressChar(Keys key)
        {
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_CHAR, (uint)key, getlParam((uint)key, false, true));
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_CHAR, (uint)key, getlParam((uint)key, true, true));
        }
    }
}