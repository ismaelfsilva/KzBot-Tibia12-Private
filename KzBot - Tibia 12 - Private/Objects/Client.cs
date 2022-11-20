using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzBot.Objects
{
    public static class Client
    {
        private static List<uint> foundCooldownAddresses = new List<uint>();

        public static bool hasCooldown(CooldownGroup group)
        {
            uint collectionAddr = WinApi.ReadOffsetUInt32(Globals.Handle, Addresses.Player.Pointer, new uint[] { 0x74, 0x18 });
            List<uint> collection = Util.QtCollectionHelper.Read(collectionAddr);

            foreach (uint item in collection)
            {
                int id = WinApi.ReadByte(Globals.Handle, item + 0x10);

                if (id == (int)group)
                    return WinApi.ReadInt64(Globals.Handle, item + 0x18) == 0;
            }

            return false;
        }
        

        public static void targetNear()
        {
            List<Creature> creatures = Battlelist.getCreaturesOnScreen().FindAll(cr => cr.Type == CreatureType.Monster && cr.HealthPc > 0 && !Globals.Config.ignore_List.Contains(cr.Name));
            if (creatures.Count <= 0)
                return;

            targetNear(creatures);
        }

        public static void targetNear(List<Creature> creatures)
        {
            try
            {
                if (creatures.Count <= 0)
                    return;

                Position playerPos = Objects.Player.Position;
                Creature playerTarget = creatures.Find(c => c.Id == Player.TargetId);
                if (Globals.Config.ignore_List.Contains(playerTarget?.Name))
                    playerTarget = null;

                int distToTarget = playerTarget != null ? playerTarget.Position.distanceTo(playerPos) : 50;
                Creature creatureToTarget = null;
                if (distToTarget > 1)
                {
                    foreach (Creature cr in creatures)
                    {
                        int distToCreature = cr.Position.distanceTo(playerPos);

                        if (distToCreature < distToTarget)
                        {
                            distToTarget = distToCreature;
                            creatureToTarget = cr;
                        }
                    }

                    if (creatureToTarget != null && creatureToTarget.Id != Player.TargetId && distToTarget <= Globals.Config.max_Distance_To_Target)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            if (Player.TargetId == creatureToTarget.Id || (Player.TargetId != 0 && creatures.Find(c => c.Id == Player.TargetId)?.Position.distanceTo(playerPos) <= distToTarget))
                            {
                                if (!Globals.Config.ignore_List.Contains(creatures.Find(c => c.Id == Player.TargetId)?.Name))
                                    break;
                            }

                            Keyboard.PressKey((Keys)Properties.Settings.Default.Target_Next_Key);
                            System.Threading.Thread.Sleep(25);
                        }
                    }
                }
            }
            catch
            {

            }            
        }

        public static bool HasHealCooldown
        {
            get
            {
                if (Addresses.Client.healCooldown == 0x0)
                    return true;

                return WinApi.ReadInt64(Globals.Handle, Addresses.Client.healCooldown) == 0;
            }
        }

        public static bool HasSupportCooldown
        {
            get
            {
                if (Addresses.Client.supportCooldown == 0x0)
                    return true;

                return WinApi.ReadInt64(Globals.Handle, Addresses.Client.supportCooldown) == 0;
            }
        }

        public static bool HasAttackCooldown
        {
            get
            {
                if (Addresses.Client.attackCooldown == 0x0)
                    return true;

                return WinApi.ReadInt64(Globals.Handle, Addresses.Client.attackCooldown) == 0;
            }
        }

        public static long Time
        {
            get
            {
                return WinApi.ReadOffsetInt64(Globals.Handle, Addresses.Player.Pointer, Addresses.Client.clientTime);
            }
        }

        public static void Say(string text)
        {
            Keyboard.PressKey(Keys.Enter);
            System.Threading.Thread.Sleep(200);
            foreach (byte b in ASCIIEncoding.Default.GetBytes(text))
            {
                Keyboard.PressChar(b);
            }
            System.Threading.Thread.Sleep(200);
            Keyboard.PressKey(Keys.Enter);
            System.Threading.Thread.Sleep(200);
        }

        public static Bitmap CaptureApplication()
        {
            WinApi.RECT rc;
            WinApi.GetWindowRect(Globals.Process.MainWindowHandle, out rc);

            Bitmap bmp = new Bitmap(rc.right - rc.left, rc.bottom - rc.top, PixelFormat.Format24bppRgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            WinApi.PrintWindow(Globals.Handle, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }



        public static void dragMouse(Point from, Point to)
        {
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 1, (uint)WinApi.MakeLParam(from.X, from.Y));
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_LBUTTONDOWN, 1, (uint)WinApi.MakeLParam(from.X, from.Y));
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 1, (uint)WinApi.MakeLParam(to.X, to.Y));
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_LBUTTONUP, 0, (uint)WinApi.MakeLParam(to.X, to.Y));
        }

        public static void leftClick(int x, int y)
        {
            uint lParam = (uint)WinApi.MakeLParam(x, y);
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_LBUTTONDOWN, WinApi.MK_LBUTTON, lParam);
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_LBUTTONUP, 0, lParam);
        }

        public static void moveMouse(int x, int y)
        {
            uint lParam = (uint)WinApi.MakeLParam(x, y);
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
        }

        public static void mouseWheel(int x, int y, bool up)
        {
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 1, (uint)WinApi.MakeLParam(x, y));
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEWHEEL, (up) ? 0x780000 : 0xFF880000, Convert.ToUInt32(y * 65536 + x));
            //zDelta -120 -> Down
            //zDelta 120 -> Up
        }

        public static void rightClickPos(int x, int y)
        {
            uint lParam = (uint)WinApi.MakeLParam(x, y);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_RBUTTONDOWN, WinApi.MK_RBUTTON, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, WinApi.MK_RBUTTON, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_RBUTTONUP, 0, lParam);
        }

        public static void middleClickPos(int x, int y)
        {
            // Execute click
            uint lParam = (uint)WinApi.MakeLParam(x, y);
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_MBUTTONDOWN, WinApi.MK_MBUTTON, 0);
            //WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
            WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_MBUTTONUP, 0, 0);
        }

        public static void SetCooldownAddresses()
        {
            uint collectionAddr = WinApi.ReadOffsetUInt32(Globals.Handle, Addresses.Player.Pointer, new uint[] { 0x74, 0x18 });
            List<uint> collection = Util.QtCollectionHelper.Read(collectionAddr);

            foreach (uint item in collection)
            {
                int id = WinApi.ReadByte(Globals.Handle, item + 0x10);

                Debug.WriteLine(id);

                if (id == 1)
                    Addresses.Client.attackCooldown = item + 0x18;
                else if (id == 2)
                    Addresses.Client.healCooldown = item + 0x18;
                else if (id == 3)
                    Addresses.Client.supportCooldown = item + 0x18;
            }
        }

        public static int getItemCount(int itemId)
        {
            try
            {
                uint collectionAddr = WinApi.ReadOffsetUInt32(Globals.Handle, Addresses.Player.Pointer, new uint[] { 0xA0, 0x18, 0x20 });
                List<uint> collection = Util.QtCollectionHelper.Read(collectionAddr);

                foreach (uint item in collection)
                {
                    long id = WinApi.ReadInt16(Globals.Handle, item + 0x10);

                    Debug.WriteLine("Id: {0} | Count: {1}", id, WinApi.ReadInt16(Globals.Handle, item + 0x24));

                    if (id == itemId)
                        return WinApi.ReadInt16(Globals.Handle, item + 0x24);
                }
            }
            catch (Exception ex)
            { }

            return 0;
        }

        public static int getItemCount(string itemName)
        {
            try
            {
                uint collectionAddr = WinApi.ReadOffsetUInt32(Globals.Handle, Addresses.Player.Pointer, new uint[] { 0xA0, 0x18, 0x20 });
                List<uint> collection = Util.QtCollectionHelper.Read(collectionAddr);

                foreach (uint item in collection)
                {
                    uint namePointer = WinApi.ReadUInt32(Globals.Handle, item + 0x28);
                    string name = WinApi.ReadString(Globals.Handle, namePointer + 0x10);

                    if (name == itemName)
                        return WinApi.ReadInt16(Globals.Handle, item + 0x24);
                }
            }
            catch (Exception ex)
            { }

            return 0;
        }
    }
}
