using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KzBot.Objects
{
    public static class Client
    {
        private static List<uint> foundCooldownAddresses = new List<uint>();

        public static List<Point> equipmentPoints = new List<Point>()
        {
            new Point(-150, 170),
            new Point(-115, 170),
            new Point(-75, 170),
            new Point(-150, 205),
            new Point(-115, 205),
            new Point(-75, 205),
            new Point(-150, 240),
            new Point(-115, 240),
            new Point(-75, 240),
            new Point(-115, 270),
        };

        public static void lookAt(Equipment equipment)
        {
            Point equipPoint = equipmentPoints[(int)equipment];

            lookClick(Globals.clientRect.right - Globals.clientRect.left + equipPoint.X, equipPoint.Y);
        }

        public static void takeOut(Equipment equipment)
        {
            Position playerPos = Objects.Player.Position;
            takeOut(equipment, playerPos);
            
        }

        public static void takeOut(Equipment equipment, Position pos)
        {
            if (!Globals.ScriptConfig.GeneralStatus || Globals.Process == null || Globals.Process.HasExited || !Objects.Player.isLoggedIn || !Objects.Player.isAlive())
                return;

            Point equipPoint = equipmentPoints[(int)equipment];
            Point equipFinalPoint = new Point(Globals.clientRect.right - Globals.clientRect.left + equipPoint.X, equipPoint.Y);

            Position playerPos = Objects.Player.Position;
            Point sqmPosition = new Point();
            sqmPosition.X = Objects.ClientData.GameMapCenter.X + ((pos.X - playerPos.X) * Objects.ClientData.SqmSize.Width);
            sqmPosition.Y = Objects.ClientData.GameMapCenter.Y + ((pos.Y - playerPos.Y) * Objects.ClientData.SqmSize.Height);

            Client.dragMouse(equipFinalPoint, sqmPosition);
            System.Threading.Thread.Sleep(1000);
        }

            public static void buyItem(string itemName, int qty, Position pos)
        {
            if (!Globals.ScriptConfig.GeneralStatus || Globals.Process == null || Globals.Process.HasExited || !Objects.Player.isLoggedIn || !Objects.Player.isAlive())
                return;

            // MAKE SURE CAN TYPE ON WINDOW
            WinApi.WindowPlacement placement = new WinApi.WindowPlacement();
            WinApi.GetWindowPlacement(Globals.Process.MainWindowHandle, ref placement);
            bool changedFocus = false;

            if (placement.showCmd == 2)
            {
                changedFocus = true;
                WinApi.ShowWindow(Globals.Process.MainWindowHandle, 4);
            }

            // CLOSE ALL WINDOWS AFTER BATTLELIST
            Point closeWindow = new Point(Globals.clientRect.right - 8, 510);
            for (int i = 0; i < 10; i++)
            {
                Client.leftClick(closeWindow.X, closeWindow.Y);
                System.Threading.Thread.Sleep(100);
            }
            System.Threading.Thread.Sleep(1000);

            // OPEN DEPOT
            Position playerPos = Objects.Player.Position;
            Point sqmPosition = new Point();
            sqmPosition.X = Objects.ClientData.GameMapCenter.X + ((pos.X - playerPos.X) * Objects.ClientData.SqmSize.Width);
            sqmPosition.Y = Objects.ClientData.GameMapCenter.Y + ((pos.Y - playerPos.Y) * Objects.ClientData.SqmSize.Height);
            Objects.Client.rightClickPos(sqmPosition.X, sqmPosition.Y);
            System.Threading.Thread.Sleep(1000);

            // GET POINTS
            Point middleScreenPoint = new Point((Globals.clientRect.right - Globals.clientRect.left) / 2, (Globals.clientRect.bottom - Globals.clientRect.top) / 2);
            Point marketPoint = new Point(Globals.clientRect.right - 40,535);
            Point mailBoxPoint = new Point(Globals.clientRect.right - 75, 535);
            Point mailBoxFirstItemPoint = new Point(Globals.clientRect.right - 145, 535);
            Point backpackRelativePoint = Objects.Client.equipmentPoints[(int)Equipment.Backpack];
            Point backpackPoint = new Point(Globals.clientRect.right - Globals.clientRect.left + backpackRelativePoint.X, backpackRelativePoint.Y);

            Point resetTextButtonPoint = new Point(middleScreenPoint.X - 205, middleScreenPoint.Y + 205);
            Point enterTextButtonPoint = new Point(middleScreenPoint.X - 300, middleScreenPoint.Y + 205);

            Point firstItemPoint = new Point(middleScreenPoint.X - 300, middleScreenPoint.Y - 55);
            Point firstOfferPoint = new Point(middleScreenPoint.X, middleScreenPoint.Y - 185);

            Point increaseQtyPoint = new Point(middleScreenPoint.X + 165, middleScreenPoint.Y - 225);
            Point accepButtonPoint = new Point(middleScreenPoint.X + 335, middleScreenPoint.Y - 225);

            // DO STUFF
            Client.rightClickPos(marketPoint.X, marketPoint.Y);
            System.Threading.Thread.Sleep(1000);
            Client.leftClick(resetTextButtonPoint.X, resetTextButtonPoint.Y);
            System.Threading.Thread.Sleep(1000);
            Client.leftClick(enterTextButtonPoint.X, enterTextButtonPoint.Y);
            System.Threading.Thread.Sleep(1000);
            Keyboard.Write(itemName);
            System.Threading.Thread.Sleep(1000);
            Client.leftClick(firstItemPoint.X, firstItemPoint.Y);
            System.Threading.Thread.Sleep(1000);
            Client.leftClick(firstOfferPoint.X, firstOfferPoint.Y);
            System.Threading.Thread.Sleep(1000);
            for (int i = 1; i < qty; i++)
            {
                Client.leftClick(increaseQtyPoint.X, increaseQtyPoint.Y);
                System.Threading.Thread.Sleep(250);
            }
            System.Threading.Thread.Sleep(750);
            Client.leftClick(accepButtonPoint.X, accepButtonPoint.Y);
            System.Threading.Thread.Sleep(1000);
            Keyboard.PressKey(Keys.Escape);
            System.Threading.Thread.Sleep(1000);
            Client.rightClickPos(mailBoxPoint.X, mailBoxPoint.Y);
            System.Threading.Thread.Sleep(1000);
            Client.dragMouse(mailBoxFirstItemPoint, backpackPoint);
            System.Threading.Thread.Sleep(1000);

            // MINIMIZE WINDOW IF IT HAS TO
            if (changedFocus)
                WinApi.ShowWindow(Globals.Process.MainWindowHandle, 2);
        }

        public static void doImbue(Equipment equipment, Position pos, int slot, int imbueId, int imbueTier = 3)
        {
            if (!Globals.ScriptConfig.GeneralStatus || Globals.Process == null || Globals.Process.HasExited || !Objects.Player.isLoggedIn || !Objects.Player.isAlive())
                return;

            // MAKE SURE CAN TYPE ON WINDOW
            WinApi.WindowPlacement placement = new WinApi.WindowPlacement();
            WinApi.GetWindowPlacement(Globals.Process.MainWindowHandle, ref placement);
            bool changedFocus = false;

            if (placement.showCmd == 2)
            {
                changedFocus = true;
                WinApi.ShowWindow(Globals.Process.MainWindowHandle, 4);
            }

            Point middleScreenPoint = new Point((Globals.clientRect.right - Globals.clientRect.left) / 2, (Globals.clientRect.bottom - Globals.clientRect.top) / 2);

            Point itemRelativePoint = Objects.Client.equipmentPoints[(int)equipment];
            Point itemPoint = new Point(Globals.clientRect.right - Globals.clientRect.left + itemRelativePoint.X, itemRelativePoint.Y);
            Point backpackFirstItemPoint = new Point(Globals.clientRect.right - 145, 535);
            Point backpackRelativePoint = Objects.Client.equipmentPoints[(int)Equipment.Backpack];
            Point backpackPoint = new Point(Globals.clientRect.right - Globals.clientRect.left + backpackRelativePoint.X, backpackRelativePoint.Y);

            List<Point> slotPoints = new List<Point> {
            new Point(middleScreenPoint.X + 55, middleScreenPoint.Y - 180),
            new Point(middleScreenPoint.X + 140, middleScreenPoint.Y - 180),
            new Point(middleScreenPoint.X + 230, middleScreenPoint.Y - 180),
        };

            Point imbueTypeList = new Point(middleScreenPoint.X - 50, middleScreenPoint.Y - 85);
            Point imbueTierList = new Point(middleScreenPoint.X + 50, middleScreenPoint.Y - 85);

            Point improveChance = new Point(middleScreenPoint.X -10, middleScreenPoint.Y + 60);
            Point doImbue = new Point(middleScreenPoint.X + 200, middleScreenPoint.Y + 60);

            // CLOSE ALL WINDOWS AFTER BATTLELIST
            Point closeWindow = new Point(Globals.clientRect.right - 8, 510);
            for (int i = 0; i < 10; i++)
            {
                Client.leftClick(closeWindow.X, closeWindow.Y);
                System.Threading.Thread.Sleep(100);
            }
            System.Threading.Thread.Sleep(1000);

            // PUT ITEM IN BACKPACK
            Client.dragMouse(itemPoint, backpackPoint);
            System.Threading.Thread.Sleep(1000);

            // OPEN BACKPACK
            Client.rightClickPos(backpackPoint);
            System.Threading.Thread.Sleep(1000);

            // OPEN IMBUE WINDOW
            Position playerPos = Objects.Player.Position;
            Point sqmPosition = new Point();
            sqmPosition.X = Objects.ClientData.GameMapCenter.X + ((pos.X - playerPos.X) * Objects.ClientData.SqmSize.Width);
            sqmPosition.Y = Objects.ClientData.GameMapCenter.Y + ((pos.Y - playerPos.Y) * Objects.ClientData.SqmSize.Height);
            Objects.Client.rightClickPos(sqmPosition.X, sqmPosition.Y);
            System.Threading.Thread.Sleep(1000);
            Objects.Client.leftClick(backpackFirstItemPoint);
            System.Threading.Thread.Sleep(1000);

            // SELECT SLOT
            Client.leftClick(slotPoints[slot-1]);
            System.Threading.Thread.Sleep(1000);

            // SELECT TYPE
            Client.leftClick(imbueTypeList);
            System.Threading.Thread.Sleep(1000);
            for (int i = 1; i < imbueId; i++)
            {
                Keyboard.PressKey(Keys.Down);
                System.Threading.Thread.Sleep(250);
            }
            System.Threading.Thread.Sleep(750);
            Keyboard.PressKey(Keys.Enter);
            System.Threading.Thread.Sleep(1000);

            // SELECT TIER
            Client.leftClick(imbueTierList);
            System.Threading.Thread.Sleep(1000);
            for (int i = 1; i < imbueTier; i++)
            {
                Keyboard.PressKey(Keys.Down);
                System.Threading.Thread.Sleep(250);
            }
            System.Threading.Thread.Sleep(750);
            Keyboard.PressKey(Keys.Enter);
            System.Threading.Thread.Sleep(1000);

            // IMPROVE CHANCE
            Client.leftClick(improveChance);
            System.Threading.Thread.Sleep(1000);

            // DO IMBUE
            Client.leftClick(doImbue);
            System.Threading.Thread.Sleep(1000);
            Keyboard.PressKey(Keys.Enter);
            System.Threading.Thread.Sleep(1000);

            // MINIMIZE WINDOW IF IT HAS TO
            if (changedFocus)
                WinApi.ShowWindow(Globals.Process.MainWindowHandle, 2);
        }

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

        public static List<string> getNpcMessages()
        {
            string clipboard = Clipboard.GetText();
            Globals.Main.Invoke((MethodInvoker)delegate
            {
                clipboard = Clipboard.GetText();
            });

            string serverLogText = string.Empty;
            List<string> messages = new List<string>();

            Keyboard.PressKey(Keys.F22);
            Keyboard.PressKey(Keys.F23);
            Keyboard.PressKey(Keys.F24);
            Keyboard.PressKey(Keys.Enter);
            System.Threading.Thread.Sleep(100);


            Globals.Main.Invoke((MethodInvoker)delegate
            {
                serverLogText = Clipboard.GetText();

                Clipboard.Clear();
                if (clipboard != String.Empty)
                    Clipboard.SetText(clipboard);
            });


            System.Threading.Thread.Sleep(1000);


            using (StringReader reader = new StringReader(serverLogText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    messages.Add(line);
                }
            }

            return messages;
        }

        public static List<string> getServerLogMessages()
        {
            string clipboard = Clipboard.GetText();
            Globals.Main.Invoke((MethodInvoker)delegate
            {
                clipboard = Clipboard.GetText();
            });

            string serverLogText = string.Empty;
            List<string> messages = new List<string>();

            Keyboard.PressKey(Keys.F21);
            Keyboard.PressKey(Keys.F23);
            Keyboard.PressKey(Keys.F24);
            Keyboard.PressKey(Keys.Enter);
            System.Threading.Thread.Sleep(100);


            Globals.Main.Invoke((MethodInvoker)delegate
            {
                serverLogText = Clipboard.GetText();

                Clipboard.Clear();
                if (clipboard != String.Empty)
                    Clipboard.SetText(clipboard);
            });


            System.Threading.Thread.Sleep(1000);


            using (StringReader reader = new StringReader(serverLogText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    messages.Add(line);
                }
            }

            return messages;
        }



        public static void targetNear()
        {
            List<Creature> creatures = Battlelist.getCreaturesOnScreen().FindAll(cr => cr.Type == CreatureType.Monster && cr.HealthPc > 0 && !Globals.ScriptConfig.ignore_List.Contains(cr.Name));
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
                if (Globals.ScriptConfig.ignore_List.Contains(playerTarget?.Name))
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

                    if (creatureToTarget != null && creatureToTarget.Id != Player.TargetId && distToTarget <= Globals.ScriptConfig.max_Distance_To_Target)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            if (Player.TargetId == creatureToTarget.Id || (Player.TargetId != 0 && creatures.Find(c => c.Id == Player.TargetId)?.Position.distanceTo(playerPos) <= distToTarget))
                            {
                                if (!Globals.ScriptConfig.ignore_List.Contains(creatures.Find(c => c.Id == Player.TargetId)?.Name))
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
            Keyboard.PressKey(Keys.F19);
            Keyboard.PressKey(Keys.Enter);
            System.Threading.Thread.Sleep(200);
            foreach (byte b in ASCIIEncoding.Default.GetBytes(text))
            {
                Keyboard.PressChar(b);
            }
            System.Threading.Thread.Sleep(200);
            Keyboard.PressKey(Keys.Enter);
            System.Threading.Thread.Sleep(200);
            Keyboard.PressKey(Keys.F20);
        }

        public static Bitmap CaptureApplication()
        {
            WinApi.RECT rc = Globals.clientRect;

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
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, (uint)WinApi.MakeLParam(from.X, from.Y));
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_LBUTTONDOWN, WinApi.MK_LBUTTON, (uint)WinApi.MakeLParam(from.X, from.Y));
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, WinApi.MK_LBUTTON, (uint)WinApi.MakeLParam(to.X, to.Y));
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_LBUTTONUP, 0, (uint)WinApi.MakeLParam(to.X, to.Y));
        }

        public static void leftClick(Point p)
        {
            leftClick(p.X, p.Y);
        }

        public static void leftClick(int x, int y)
        {
            uint lParam = (uint)WinApi.MakeLParam(x, y);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_LBUTTONDOWN, WinApi.MK_LBUTTON, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_LBUTTONUP, 0, lParam);
        }

        public static void lookClick(int x, int y)
        {
            uint lParam = (uint)WinApi.MakeLParam(x, y);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_LBUTTONDOWN, WinApi.MK_LBUTTON, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_RBUTTONDOWN, WinApi.MK_LBUTTON | WinApi.MK_RBUTTON, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_RBUTTONUP, WinApi.MK_LBUTTON, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_LBUTTONUP, 0, lParam);
        }

        public static void moveMouse(int x, int y)
        {
            uint lParam = (uint)WinApi.MakeLParam(x, y);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
        }

        public static void mouseWheel(int x, int y, bool up)
        {
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 1, (uint)WinApi.MakeLParam(x, y));
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEWHEEL, (up) ? 0x780000 : 0xFF880000, Convert.ToUInt32(y * 65536 + x));
            //zDelta -120 -> Down
            //zDelta 120 -> Up
        }
        public static void rightClickPos(Point p)
        {
            rightClickPos(p.X, p.Y);
        }

            public static void rightClickPos(int x, int y)
        {
            uint lParam = (uint)WinApi.MakeLParam(x, y);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_RBUTTONDOWN, WinApi.MK_RBUTTON, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_RBUTTONUP, 0, lParam);
        }

        public static void middleClickPos(int x, int y)
        {
            // Execute click
            uint lParam = (uint)WinApi.MakeLParam(x, y);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MBUTTONDOWN, WinApi.MK_MBUTTON, 0);
            //WinApi.PostMessage(Globals.Process.MainWindowHandle, WinApi.WM_MOUSEMOVE, 0, lParam);
            WinApi.SendMessage(Globals.Process.MainWindowHandle, WinApi.WM_MBUTTONUP, 0, 0);
        }

        public static void SetCooldownAddresses()
        {
            uint collectionAddr = WinApi.ReadOffsetUInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.cooldowns);
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
                uint collectionAddr = WinApi.ReadOffsetUInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.items);
                List<uint> collection = Util.QtCollectionHelper.Read(collectionAddr);

                foreach (uint item in collection)
                {
                    long id = WinApi.ReadUInt16(Globals.Handle, item + 0x10);

                    //Debug.WriteLine("Id: {0} | Count: {1} | Addr: {2}", id, WinApi.ReadInt16(Globals.Handle, item + 0x24), item.ToString("X"));

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
                uint collectionAddr = WinApi.ReadOffsetUInt32(Globals.Handle, Addresses.Player.Pointer, Addresses.Player.items);
                List<uint> collection = Util.QtCollectionHelper.Read(collectionAddr);

                foreach (uint item in collection)
                {
                    uint namePointer = WinApi.ReadUInt32(Globals.Handle, item + 0x28);
                    string name = WinApi.ReadString(Globals.Handle, namePointer + 0x10);

                    if (name.ToLower() == itemName.ToLower())
                        return WinApi.ReadInt16(Globals.Handle, item + 0x24);
                }
            }
            catch (Exception ex)
            { }

            return 0;
        }
    }
}
