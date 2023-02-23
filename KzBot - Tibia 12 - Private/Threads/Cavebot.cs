using KzBot.Objects;
using KzBot.UI;
using KzBot.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace KzBot.Threads
{
    public static class Cavebot
    {
        public static System.Threading.Timer Thread = new System.Threading.Timer(CavebotThread, null, Timeout.Infinite, Timeout.Infinite);
        public static bool didWaitBecauseOfPlayerOnCombo = false;
        public static DateTime lastLureStart = DateTime.MinValue;
        public static int lastCharacterId = 0;
        public static string lastStatus = "None";

        public static DateTime idleUntil = DateTime.MinValue;

        private static void CavebotThread(object state)
        {
            Thread.Change(Timeout.Infinite, Timeout.Infinite);
            int startWaypointId = Globals.WaypointId;

            bool healerStatus = Globals.ScriptConfig.HealerStatus;
            bool targetStatus = Globals.ScriptConfig.TargetingStatus;
            bool hasteStatus = Globals.ScriptConfig.auto_Haste;
            bool changedStatus = false;
            bool isChatOn = false;
            bool changedFocus = false;

            bool instantSkip = false;

            try
            {
                if (Globals.ScriptConfig.Waypoints.Count <= 0)
                    return;

                if (Globals.ScriptConfig.Waypoints.Count > 0 && Globals.WaypointId >= Globals.ScriptConfig.Waypoints.Count)
                    Globals.WaypointId = 0;

                startWaypointId = Globals.WaypointId;
                Waypoint waypoint = Globals.ScriptConfig.Waypoints[Globals.WaypointId];

                if (waypoint.Type == WaypointType.Login_Next)
                {

                }
                else if (waypoint.Type == WaypointType.Setup_Config)
                {
                    int charId = lastCharacterId;
                    if (charId <= 0)
                    {
                        Globals.WaypointId++;
                        return;
                    }

                    try
                    {
                        string botPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
                        string lootFile = "lootBlackWhitelist.json";
                        string sidebarsFile = "sidebars.json";
                        File.Copy(botPath + lootFile, Globals.Server.path + @"\characterdata\" + charId + @"\" + lootFile, true);
                        File.Copy(botPath + sidebarsFile, Globals.Server.path + @"\characterdata\" + charId + @"\" + sidebarsFile, true);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else if (waypoint.Type == WaypointType.Close_Bot)
                {
                    Globals.Main.Log.addLog("Closing Bot", false);
                    Threads.ClientData.UpdateCharacter();

                    System.Threading.Thread.Sleep(5000);

                    Globals.Main.Invoke((MethodInvoker)delegate
                    {
                        Globals.Main.canCloseForm = true;
                        Globals.Main.Close();
                    });
                    return;
                }
                else if (waypoint.Type == WaypointType.Exit)
                {
                    Globals.Main.Log.addLog("Exiting Character", false);
                    Threads.ClientData.UpdateCharacter();

                    System.Threading.Thread.Sleep(5000);

                    Globals.Process.Kill(true);
                    Globals.Process = null;

                    Globals.WaypointId++;
                }

                if (!Globals.ScriptConfig.GeneralStatus || !Globals.ScriptConfig.CavebotStatus || Globals.Process == null || Globals.Process.HasExited || !Objects.Player.isLoggedIn || !Objects.Player.isAlive())
                    return;


                if (Globals.ScriptConfig.stop_Walking_on_Target && (Objects.Player.isAttacking || Threads.Targeting.creaturesToTarget > 1))
                    return;

                Position playerPos = Objects.Player.Position;

                if (waypoint == null)
                    return;

                switch (waypoint.Type)
                {
                    // WAYPOINTS THAT DON'T NEED THE PLAYER TO BE IN RANGE TO EXECUTE
                    case WaypointType.If_Location_Goto_Label:
                    case WaypointType.If_Location_Goback:
                        break;
                    case WaypointType.Stand:
                    case WaypointType.Node:
                    case WaypointType.Use:
                    case WaypointType.Use_On:
                    case WaypointType.Throw_First_Slot:
                    case WaypointType.Imbue:
                    case WaypointType.Go_Near:
                    case WaypointType.Teleport:
                    case WaypointType.Take_Out_Equip:
                    case WaypointType.Step:
                        if (playerPos.Z != waypoint.Z)
                        {
                            Globals.WaypointId++;
                            instantSkip = true;
                            return;
                        }
                        break;
                    case WaypointType.Not_Location_Goto_Label:
                    case WaypointType.Not_Location_Goback:
                        if (Math.Abs(playerPos.X - waypoint.X) >= 200 && Math.Abs(playerPos.Y - waypoint.Y) >= 200)
                        {
                            Globals.WaypointId++;
                            instantSkip = true;
                            return;
                        }
                        break;
                    default:
                        if (!(waypoint.X == 0 && waypoint.Y == 0 && waypoint.Z == 0) && Math.Abs(playerPos.X - waypoint.X) >= waypoint.rangeX && Math.Abs(playerPos.Y - waypoint.Y) >= waypoint.rangeY)
                        {
                            Globals.WaypointId++;
                            instantSkip = true;
                            return;
                        }
                        break;
                }


                //if (Player.isWalking && (Math.Abs(Player.GotoX - waypoint.X) < waypoint.rangeX && Math.Abs(Player.GotoY - waypoint.Y) < waypoint.rangeY && Player.GotoZ == waypoint.Z))
                //    return;

                string[] extraData = waypoint.Extra.Split(";");

                switch (waypoint.Type)
                {
                    case WaypointType.Stand:
                        if (Math.Abs(playerPos.X - waypoint.X) < waypoint.rangeX && Math.Abs(playerPos.Y - waypoint.Y) < waypoint.rangeY || playerPos.Z != waypoint.Z)
                        {
                            Globals.WaypointId++;
                            instantSkip = true;
                            if (waypoint.rangeX == 1 && waypoint.rangeY == 1)
                                Keyboard.PressKey(Keys.Escape);
                        }
                        else if (Math.Abs(playerPos.X - waypoint.X) > 200 || Math.Abs(playerPos.Y - waypoint.Y) > 200)
                        {
                            Globals.WaypointId++;
                            instantSkip = true;
                        }
                        else
                        {
                            Player.Goto(waypoint.X, waypoint.Y, waypoint.Z);
                            System.Threading.Thread.Sleep(Math.Min(500, 100 * (waypoint.Position.distanceTo(playerPos) - 1)));
                        }
                        break;
                    case WaypointType.Node:
                        if (Math.Abs(playerPos.X - waypoint.X) < waypoint.rangeX && Math.Abs(playerPos.Y - waypoint.Y) < waypoint.rangeY || playerPos.Z != waypoint.Z)
                        {
                            Globals.WaypointId++;
                            instantSkip = true;
                        }
                        else if (Math.Abs(playerPos.X - waypoint.X) > 200 || Math.Abs(playerPos.Y - waypoint.Y) > 200)
                        {
                            Globals.WaypointId++;
                            instantSkip = true;
                        }
                        else if (Math.Abs(playerPos.X - Objects.Player.GotoX) >= waypoint.rangeX || Math.Abs(playerPos.Y - Objects.Player.GotoY) >= waypoint.rangeY)
                        {
                            int gotoX = waypoint.X + new Random().Next(waypoint.rangeX * -1 + 1, waypoint.rangeX - 1);
                            int gotoY = waypoint.Y + new Random().Next(waypoint.rangeY * -1 + 1, waypoint.rangeY - 1);
                            Player.Goto(gotoX, gotoY, waypoint.Z);
                            System.Threading.Thread.Sleep(Math.Min(500, 100 * (new Position() { X = gotoX, Y=gotoY, Z=waypoint.Z} .distanceTo(playerPos) - 1)));
                        }
                        break;
                    case WaypointType.Say:
                        {
                            changedStatus = true;

                            Globals.Main.Invoke((MethodInvoker)delegate {
                                Globals.Main.checkBox2.Checked = false; // HEALER
                                Globals.Main.checkBox4.Checked = false; // TARGETING
                            });
                            Globals.ScriptConfig.auto_Haste = false;

                            Keyboard.PressKey(Keys.F19);
                            System.Threading.Thread.Sleep(50);

                            Client.Say(waypoint.Extra);

                            System.Threading.Thread.Sleep(50);
                            Keyboard.PressKey(Keys.F20);

                            Globals.WaypointId++;
                            instantSkip = true;
                            break;

                        }
                    case WaypointType.Wait:
                        idleUntil = DateTime.Now.AddMilliseconds(int.Parse(waypoint.Extra));
                        System.Threading.Thread.Sleep(int.Parse(waypoint.Extra));
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Lure:
                        {
                            if (!Globals.ComboStatus && waypoint.Extra.Trim().Length > 0)
                                System.Threading.Thread.Sleep(int.Parse(waypoint.Extra));

                            List<Creature> creatures = Battlelist.getCreaturesOnScreen().FindAll(cr => cr.HealthPc > 0 && !Globals.ScriptConfig.ignore_List.Contains(cr.Name));
                            if (Globals.ScriptConfig.check_Only_Near_Creatures_If_Player_on_Screen && creatures.Exists(c => c.Type == CreatureType.Player && c.Address != Player.Creature.Address))
                            {
                                if (!didWaitBecauseOfPlayerOnCombo)
                                {
                                    System.Threading.Thread.Sleep(Globals.ScriptConfig.time_To_Wait_Before_Checking_Creatures_If_Player_on_Screen);
                                    didWaitBecauseOfPlayerOnCombo = true;
                                }

                                playerPos = Objects.Player.Position;
                                creatures.RemoveAll(c => Math.Abs(c.X - playerPos.X) > 1 || Math.Abs(c.Y - playerPos.Y) > 1);
                            }

                            creatures.RemoveAll(c => c.Type != CreatureType.Monster);

                            if (!Globals.ComboStatus && creatures.Count >= Globals.ScriptConfig.creature_Count_To_Skip_Lure)
                            {
                                lastLureStart = DateTime.Now;
                                Globals.ComboStatus = true;
                            }
                            else if (Globals.ScriptConfig.max_Lure_Time > 0 && (DateTime.Now - lastLureStart).TotalMilliseconds >= Globals.ScriptConfig.max_Lure_Time)
                            {
                                Globals.ComboStatus = false;
                                didWaitBecauseOfPlayerOnCombo = false;
                                instantSkip = true;
                                Globals.WaypointId++;
                            }
                            else if (Globals.ComboStatus && creatures.Count <= Globals.ScriptConfig.creature_Count_To_End_Lure - creatures.Where(c => c.HealthPc <= Globals.ScriptConfig.creatures_Left_Health_To_End_Lure).Count())
                            {
                                Globals.ComboStatus = false;
                                didWaitBecauseOfPlayerOnCombo = false;
                                instantSkip = true;
                                Globals.WaypointId++;
                            }
                            else if (!Globals.ComboStatus)
                            {
                                Globals.WaypointId++;
                                instantSkip = true;
                                didWaitBecauseOfPlayerOnCombo = false;
                                if (Globals.ScriptConfig.Waypoints[Globals.WaypointId].Type == WaypointType.Loot)
                                    Globals.WaypointId++;
                            }

                            break;

                        }
                    case WaypointType.Loot:
                        if (!Globals.HasAutoLoot)
                        {
                            List<Point> lootPointList = new List<Point>();

                            for (int x = -1; x <= 1; x++)
                            {
                                for (int y = -1; y <= 1; y++)
                                {
                                    Point sqmPosition = new Point();
                                    sqmPosition.X = Objects.ClientData.GameMapCenter.X + x * Objects.ClientData.SqmSize.Width;
                                    sqmPosition.Y = Objects.ClientData.GameMapCenter.Y + y * Objects.ClientData.SqmSize.Height;
                                    lootPointList.Add(sqmPosition);
                                }
                            }

                            for (int tries = 0; tries < 3; tries++)
                            {
                                foreach (Point p in lootPointList)
                                {
                                    Objects.Client.rightClickPos(p);
                                    System.Threading.Thread.Sleep(10);
                                }
                            }
                        }
                        else
                            Keyboard.PressKey((Keys)Properties.Settings.Default.Loot_Key);

                        Globals.WaypointId++;
                        instantSkip = true;
                        break;
                    case WaypointType.Goto_Label:
                        Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label.Trim() == waypoint.Extra.Trim());
                        instantSkip = true;
                        break;
                    case WaypointType.Check_Cap:
                        instantSkip = true;
                        if (Objects.Player.Level < Globals.ScriptConfig.min_Level_To_Check_Cap)
                            Globals.WaypointId++;
                        else if (Objects.Player.Cap < int.Parse(extraData[0]))
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == extraData[1].Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Level:
                        instantSkip = true;
                        if (Objects.Player.Level < int.Parse(extraData[0]))
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == extraData[1].Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Balance:
                        instantSkip = true;
                        if (Globals.AccMaxBalance != -1 && Threads.ClientData.lastBalance >= Globals.AccMaxBalance)
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == waypoint.Extra.Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Stamina:
                        instantSkip = true;
                        if (Objects.Player.Stamina.TotalSeconds < int.Parse(extraData[0]) * 60)
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == extraData[1].Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Safe:
                        instantSkip = true;
                        if (Threads.Alarms.safeMode)
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == waypoint.Extra.Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_PZ:
                        instantSkip = true;
                        if (Objects.Player.Creature.Skull != PlayerSkulls.White)
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == waypoint.Extra.Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Imbue:
                        {
                            instantSkip = true;
                            bool foundItem = false;
                            bool hasImbue = false;
                            Objects.Client.lookAt(Equipment.Weapon);
                            System.Threading.Thread.Sleep(500);
                            List<string> messages = Objects.Client.getServerLogMessages();
                            Regex regex = new Regex(@"void (\d+):(\d+)h");
                            for (int i = 1; i <= 25; i++)
                            {
                                if (i > messages.Count)
                                    break;

                                string msg = messages[messages.Count - i].ToLower();
                                Match match = regex.Match(msg);

                                if (msg.Contains("imbuements:"))
                                {
                                    foundItem = true;

                                    if (match.Success)
                                    {
                                        hasImbue = true;

                                        int hours = Convert.ToInt32(match.Groups[1].Value);
                                        int minutes = Convert.ToInt32(match.Groups[2].Value);

                                        Threads.ClientData.imbueTime = (hours * 60) + minutes;
                                    }

                                    break;
                                }
                            }


                            if (foundItem && !hasImbue && waypoint.Extra.Trim().ToLower() == "audio")
                            {
                                Threads.ClientData.imbueTime = 0;
                                using (var soundPlayer = new SoundPlayer(@"Sounds\Siren.wav"))
                                {
                                    soundPlayer.Play();
                                    System.Threading.Thread.Sleep(1000);
                                }
                            }
                            else if (foundItem && !hasImbue)
                            {
                                Threads.ClientData.imbueTime = 0;
                                Globals.Main.Log.addLog("Imbuement Ended", false);
                                Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == waypoint.Extra.Trim());
                            }
                            else if (foundItem)
                            {
                                Globals.WaypointId++;
                            }

                            break;
                        }
                    case WaypointType.Check_Refill:
                        {
                            instantSkip = true;
                            Globals.HasAutoLoot = Globals.Server.autoLootId != -1 && Objects.Client.getItemCount(Globals.Server.autoLootId) > 0;
                            bool needRefill = false;
                            int playerLevel = Objects.Player.Level;
                            foreach (RefillRule refill in Globals.ScriptConfig.Refill)
                            {
                                if (Globals.AccVocation != Vocation.None && refill.Vocation != Vocation.None && Globals.AccVocation != refill.Vocation)
                                    continue;

                                if ((refill.Level > 0 && playerLevel < refill.Level) || (refill.MaxLevel > 0 && playerLevel > refill.MaxLevel))
                                    continue;

                                if (Objects.Client.getItemCount(refill.Id) < refill.ToLeave)
                                {
                                    needRefill = true;
                                    break;
                                }
                            }

                            int sliverCount = Objects.Client.getItemCount(37109);
                            Threads.ClientData.lastSliverCount = sliverCount;

                            if (needRefill)
                                Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == waypoint.Extra.Trim());
                            else
                                Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Go_Near:
                        Creature cr = Objects.Battlelist.getCreaturesOnScreen().Find(c => c.Name.ToLower() == waypoint.Extra.Trim().ToLower());
                        if (cr != null && cr.Position.distanceTo(playerPos) > 1)
                            Objects.Player.Goto(cr.Position);
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Sell_All:
                        {
                            changedStatus = true;
                            Globals.Main.Invoke((MethodInvoker)delegate {
                                Globals.Main.checkBox2.Checked = false; // HEALER
                                Globals.Main.checkBox4.Checked = false; // TARGETING
                            });
                            Globals.ScriptConfig.auto_Haste = false;

                            isChatOn = true;
                            Keyboard.PressKey(Keys.F19);
                            System.Threading.Thread.Sleep(50);
                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(50);

                            Keyboard.PressKey(Keys.F22);
                            System.Threading.Thread.Sleep(50);

                            Client.Say("hi");
                            System.Threading.Thread.Sleep(50);

                            WinApi.RECT clientRect = Globals.clientRect;
                            Point closeWindow = new Point(clientRect.right - 8, 510);

                            for (int i = 0; i < 20; i++)
                            {
                                Client.leftClick(closeWindow.X, closeWindow.Y);
                                System.Threading.Thread.Sleep(10);
                            }

                            System.Threading.Thread.Sleep(50);
                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(50);
                            Client.Say("trade");
                            System.Threading.Thread.Sleep(500);

                            Point tradeWindow = new Point(clientRect.right - 155, 507);

                            Client.leftClick(tradeWindow.X + 125, tradeWindow.Y + 40);
                            System.Threading.Thread.Sleep(100);

                            int itemSoldWithoutCapChange = 0;
                            int lastCap = 0;
                            while (itemSoldWithoutCapChange <= 20)
                            {
                                // CLICK FIRST
                                Client.leftClick(tradeWindow.X + 25, tradeWindow.Y + 75);
                                System.Threading.Thread.Sleep(100);

                                Client.leftClick(tradeWindow.X + 125, tradeWindow.Y + 170);
                                System.Threading.Thread.Sleep(10);

                                itemSoldWithoutCapChange++;

                                if (itemSoldWithoutCapChange == 20)
                                {
                                    int playerCap = Objects.Player.Cap;
                                    if (lastCap != playerCap)
                                        itemSoldWithoutCapChange = 0;

                                    lastCap = playerCap;
                                }

                                if (!Globals.ScriptConfig.GeneralStatus)
                                {
                                    System.Threading.Thread.Sleep(500);
                                    Keyboard.PressKey(Keys.F20);
                                    return;
                                }
                            }
                            System.Threading.Thread.Sleep(50);
                            Keyboard.PressKey(Keys.F20);

                            Globals.WaypointId++;
                            instantSkip = true;
                            break;
                        }
                    case WaypointType.Buy_Refill:
                        {
                            changedStatus = true;
                            Globals.Main.Invoke((MethodInvoker)delegate {
                                Globals.Main.checkBox2.Checked = false; // HEALER
                                Globals.Main.checkBox4.Checked = false; // TARGETING
                            });
                            Globals.ScriptConfig.auto_Haste = false;

                            WinApi.WindowPlacement placement = new WinApi.WindowPlacement();
                            WinApi.GetWindowPlacement(Globals.Process.MainWindowHandle, ref placement);

                            if (placement.showCmd == 2)
                            {
                                changedFocus = true;
                                WinApi.ShowWindow(Globals.Process.MainWindowHandle, 4);
                            }

                            WinApi.RECT clientRect = Globals.clientRect;
                            Point closeWindow = new Point(clientRect.right - 8, 510);

                            for (int i = 0; i < 20; i++)
                            {
                                Client.leftClick(closeWindow.X, closeWindow.Y);
                                System.Threading.Thread.Sleep(10);
                            }

                            Point tradeWindow = new Point(clientRect.right - 155, 507);
                            int playerLevel = Objects.Player.Level;

                            isChatOn = true;
                            Keyboard.PressKey(Keys.F19);
                            System.Threading.Thread.Sleep(50);

                            Keyboard.PressKey(Keys.F22);
                            System.Threading.Thread.Sleep(50);

                            foreach (RefillRule refill in Globals.ScriptConfig.Refill)
                            {
                                if (Globals.AccVocation != Vocation.None && refill.Vocation != Vocation.None && Globals.AccVocation != refill.Vocation)
                                    continue;

                                if ((refill.Level > 0 && playerLevel < refill.Level) || (refill.MaxLevel > 0 && playerLevel > refill.MaxLevel))
                                    continue;

                                int itemCount = Objects.Client.getItemCount(refill.Id);

                                if (itemCount >= refill.ToBuy)
                                {
                                    continue;
                                }

                                Keyboard.PressKey(Keys.Escape);
                                System.Threading.Thread.Sleep(50);

                                Client.Say("hi");
                                System.Threading.Thread.Sleep(50);

                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                                Client.Say(refill.Type);
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                                System.Threading.Thread.Sleep(500);

                                // Click Buy
                                Client.leftClick(tradeWindow.X + 125, tradeWindow.Y + 20);
                                System.Threading.Thread.Sleep(50);

                                // Reset Item
                                Client.leftClick(tradeWindow.X + 140, tradeWindow.Y + 105);
                                System.Threading.Thread.Sleep(100);

                                // Search Item
                                Client.leftClick(tradeWindow.X + 30, tradeWindow.Y + 105);
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                                Keyboard.Write(refill.Name);
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                                System.Threading.Thread.Sleep(500);

                                // Select Item
                                Client.leftClick(tradeWindow.X + 25, tradeWindow.Y + 75);
                                System.Threading.Thread.Sleep(100);

                                // Set Count
                                Keyboard.PressKey(Keys.Escape);
                                System.Threading.Thread.Sleep(50);
                                Client.leftClick(tradeWindow.X + 95, tradeWindow.Y + 140);
                                //System.Threading.Thread.Sleep(100);
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                                Keyboard.PressKey(Keys.Delete);
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                                System.Threading.Thread.Sleep(50);
                                Client.leftClick(tradeWindow.X + 95, tradeWindow.Y + 140);
                                //System.Threading.Thread.Sleep(100);
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                                Keyboard.Write((refill.ToBuy - itemCount).ToString());
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                                System.Threading.Thread.Sleep(500);

                                // Buy
                                Client.leftClick(tradeWindow.X + 130, tradeWindow.Y + 170);
                                System.Threading.Thread.Sleep(500);

                                int newItemCount = Objects.Client.getItemCount(refill.Id);
                                if (itemCount >= newItemCount || newItemCount < refill.ToLeave)
                                    return;

                                if (!Globals.ScriptConfig.GeneralStatus)
                                {
                                    Keyboard.PressKey(Keys.F20);
                                    System.Threading.Thread.Sleep(100);

                                    return;
                                }
                            }


                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(50);


                            Keyboard.PressKey(Keys.F20);
                            System.Threading.Thread.Sleep(50);


                            Keyboard.PressKey(Keys.F12);
                            System.Threading.Thread.Sleep(50);
                            Keyboard.PressKey(Keys.F12);
                            System.Threading.Thread.Sleep(50);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Buy_Item:
                        {
                            changedStatus = true;
                            Globals.Main.Invoke((MethodInvoker)delegate {
                                Globals.Main.checkBox2.Checked = false; // HEALER
                                Globals.Main.checkBox4.Checked = false; // TARGETING
                            });
                            Globals.ScriptConfig.auto_Haste = false;

                            WinApi.WindowPlacement placement = new WinApi.WindowPlacement();
                            WinApi.GetWindowPlacement(Globals.Process.MainWindowHandle, ref placement);

                            if (placement.showCmd == 2)
                            {
                                changedFocus = true;
                                WinApi.ShowWindow(Globals.Process.MainWindowHandle, 4);
                            }

                            WinApi.RECT clientRect = Globals.clientRect;
                            Point closeWindow = new Point(clientRect.right - 8, 510);

                            for (int i = 0; i < 20; i++)
                            {
                                Client.leftClick(closeWindow.X, closeWindow.Y);
                                System.Threading.Thread.Sleep(10);
                            }

                            Point tradeWindow = new Point(clientRect.right - 155, 507);
                            int playerLevel = Objects.Player.Level;

                            isChatOn = true;
                            Keyboard.PressKey(Keys.F19);
                            System.Threading.Thread.Sleep(50);

                            Keyboard.PressKey(Keys.F22);
                            System.Threading.Thread.Sleep(50);

                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(50);

                            Client.Say("hi");
                            System.Threading.Thread.Sleep(50);

                            if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                            Client.Say(extraData[0]);
                            if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                            System.Threading.Thread.Sleep(500);

                            // Click Buy
                            Client.leftClick(tradeWindow.X + 125, tradeWindow.Y + 20);
                            System.Threading.Thread.Sleep(50);

                            // Reset Item
                            Client.leftClick(tradeWindow.X + 140, tradeWindow.Y + 105);
                            System.Threading.Thread.Sleep(100);

                            // Search Item
                            Client.leftClick(tradeWindow.X + 30, tradeWindow.Y + 105);
                            if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                            Keyboard.Write(extraData[1]);
                            if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                            System.Threading.Thread.Sleep(500);

                            // Select Item
                            Client.leftClick(tradeWindow.X + 25, tradeWindow.Y + 75);
                            System.Threading.Thread.Sleep(100);

                            // Set Count
                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(50);
                            Client.leftClick(tradeWindow.X + 95, tradeWindow.Y + 140);
                            //System.Threading.Thread.Sleep(100);
                            if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                            Keyboard.PressKey(Keys.Delete);
                            if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                            System.Threading.Thread.Sleep(50);
                            Client.leftClick(tradeWindow.X + 95, tradeWindow.Y + 140);
                            //System.Threading.Thread.Sleep(100);
                            if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                            Keyboard.Write(extraData[2]);
                            if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey) || WinApi.GetAsyncKeyState(Keys.Alt)) return;
                            System.Threading.Thread.Sleep(500);

                            // Buy
                            Client.leftClick(tradeWindow.X + 130, tradeWindow.Y + 170);
                            System.Threading.Thread.Sleep(500);

                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(50);

                            Keyboard.PressKey(Keys.F20);
                            System.Threading.Thread.Sleep(50);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Deposit_All:
                        {
                            int goldCount = Objects.Client.getItemCount(3031);
                            int platCount = Objects.Client.getItemCount(3035) * 100;
                            int crystalCount = Objects.Client.getItemCount(3043) * 10000;

                            int totalGold = goldCount + platCount + crystalCount;

                            changedStatus = true;
                            Globals.Main.Invoke((MethodInvoker)delegate {
                                Globals.Main.checkBox2.Checked = false; // HEALER
                                Globals.Main.checkBox4.Checked = false; // TARGETING
                            });
                            Globals.ScriptConfig.auto_Haste = false;

                            isChatOn = true;
                            Keyboard.PressKey(Keys.F19);
                            System.Threading.Thread.Sleep(50);
                            Keyboard.PressKey(Keys.F22);
                            System.Threading.Thread.Sleep(50);

                            Client.Say("hi");
                            System.Threading.Thread.Sleep(50);
                            Client.Say("bank");
                            System.Threading.Thread.Sleep(50);
                            Client.Say("deposit all");
                            System.Threading.Thread.Sleep(50);
                            Client.Say("yes");
                            System.Threading.Thread.Sleep(50);

                            Keyboard.PressKey(Keys.F20);
                            System.Threading.Thread.Sleep(50);

                            Globals.Main.Log.addLog("Deposited " + totalGold + " gold coins", false);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Transfer:
                        Globals.Main.Log.addLog("Transfering Cash", false);
                        Globals.WaypointId++;
                        instantSkip = true;
                        break;
                    case WaypointType.Withdraw:
                        {
                            int amountToWithdraw = 0;
                            int.TryParse(waypoint.Extra.Trim(), out amountToWithdraw);
                            if (amountToWithdraw < 0)
                                amountToWithdraw += Threads.ClientData.lastBalance;

                            amountToWithdraw = Math.Min(10000000, amountToWithdraw);

                            if (amountToWithdraw <= 0)
                            {
                                Globals.WaypointId++;
                                break;
                            }

                            if (amountToWithdraw > 100)
                                amountToWithdraw = Convert.ToInt32(Math.Floor(Convert.ToDouble(amountToWithdraw) / 10000) * 10000);

                            changedStatus = true;
                            Globals.Main.Invoke((MethodInvoker)delegate {
                                Globals.Main.checkBox2.Checked = false; // HEALER
                                Globals.Main.checkBox4.Checked = false; // TARGETING
                            });
                            Globals.ScriptConfig.auto_Haste = false;


                            isChatOn = true;
                            Keyboard.PressKey(Keys.F19);
                            System.Threading.Thread.Sleep(50);
                            Keyboard.PressKey(Keys.F22);
                            System.Threading.Thread.Sleep(50);

                            Client.Say("hi");
                            System.Threading.Thread.Sleep(50);
                            Client.Say("bank");
                            System.Threading.Thread.Sleep(50);
                            Client.Say("withdraw " + amountToWithdraw);
                            System.Threading.Thread.Sleep(50);
                            Client.Say("yes");
                            System.Threading.Thread.Sleep(50);

                            Keyboard.PressKey(Keys.F20);

                            Threads.ClientData.lastBalance -= amountToWithdraw;
                            Globals.Main.Log.addLog("Withdrawing: " + amountToWithdraw.ToString());

                            if (amountToWithdraw < 10000000)
                                Globals.WaypointId++;
                            else
                                System.Threading.Thread.Sleep(5000);

                            break;
                        }
                    case WaypointType.Balance:
                        {
                            changedStatus = true;
                            Globals.Main.Invoke((MethodInvoker)delegate {
                                Globals.Main.checkBox2.Checked = false; // HEALER
                                Globals.Main.checkBox4.Checked = false; // TARGETING
                            });
                            Globals.ScriptConfig.auto_Haste = false;


                            isChatOn = true;
                            Keyboard.PressKey(Keys.F19);
                            System.Threading.Thread.Sleep(50);
                            Keyboard.PressKey(Keys.F22);
                            System.Threading.Thread.Sleep(50);

                            Client.Say("hi");
                            System.Threading.Thread.Sleep(50);
                            Client.Say("bank");
                            System.Threading.Thread.Sleep(50);
                            Client.Say("balance");
                            System.Threading.Thread.Sleep(2000);

                            Keyboard.PressKey(Keys.F23);
                            System.Threading.Thread.Sleep(50);
                            Keyboard.PressKey(Keys.F24);
                            System.Threading.Thread.Sleep(50);

                            string balanceMessage = Objects.Client.getNpcMessages().LastOrDefault();
                            int balance = Threads.ClientData.lastBalance;
                            Regex regex = new Regex(@"\d+");

                            if (balanceMessage != null && balanceMessage.Contains("gold"))
                            {
                                MatchCollection matches = regex.Matches(balanceMessage);

                                if (matches.Count > 0)
                                    int.TryParse(matches.LastOrDefault().Value, out balance);
                            }

                            Threads.ClientData.lastBalance = balance;

                            Keyboard.PressKey(Keys.F20);
                            System.Threading.Thread.Sleep(50);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Throw_Items_Last_Bp:
                        {
                            WinApi.RECT clientRect = Globals.clientRect;
                            Point closeWindow = new Point(clientRect.right - 8, 510);

                            for (int i = 0; i < 20; i++)
                            {
                                Client.leftClick(closeWindow.X, closeWindow.Y);
                                System.Threading.Thread.Sleep(10);
                            }

                            Point backpackRelativePoint = Objects.Client.equipmentPoints[(int)Equipment.Backpack];
                            Point backpackPoint = new Point(Globals.clientRect.right - Globals.clientRect.left + backpackRelativePoint.X, backpackRelativePoint.Y);

                            Client.rightClickPos(backpackPoint);
                            System.Threading.Thread.Sleep(500);

                            Point firstSlotPoint = new Point(Globals.clientRect.right - 150, 535);

                            for (int y = 5; y > 0; y--)
                            {
                                for (int x = 4; x > 0; x--)
                                {
                                    Console.WriteLine(x + " - " + y);
                                    Point slotPoint = new Point(firstSlotPoint.X + (x - 1) * 38, firstSlotPoint.Y + (y - 1) * 38);

                                    for (int i = 1; i <= 3; i++)
                                    {
                                        Client.dragMouse(firstSlotPoint, slotPoint);
                                        System.Threading.Thread.Sleep(200);
                                    }

                                }
                            }

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Set_Label:
                        {
                            Point middleScreenPoint = new Point((Globals.clientRect.right - Globals.clientRect.left) / 2, (Globals.clientRect.bottom - Globals.clientRect.top) / 2);
                            Point okButtonPoint = new Point(middleScreenPoint.X + 70, middleScreenPoint.Y + 200);
                            Point firstSlotPoint = new Point(Globals.clientRect.right - 150, 535);
                            Point secondSlotPoint = new Point(Globals.clientRect.right - 112, 535);

                            Point backpackRelativePoint = Objects.Client.equipmentPoints[(int)Equipment.Backpack];
                            Point backpackPoint = new Point(Globals.clientRect.right - Globals.clientRect.left + backpackRelativePoint.X, backpackRelativePoint.Y);

                            Client.rightClickPos(backpackPoint);
                            System.Threading.Thread.Sleep(500);
                            Objects.Client.rightClickPos(firstSlotPoint);
                            System.Threading.Thread.Sleep(500);
                            Keyboard.Write(Globals.CharToTransfer);
                            System.Threading.Thread.Sleep(500);
                            Objects.Client.leftClick(okButtonPoint);
                            System.Threading.Thread.Sleep(500);
                            Objects.Client.dragMouse(firstSlotPoint, secondSlotPoint);
                            System.Threading.Thread.Sleep(500);
                            Objects.Client.dragMouse(secondSlotPoint, firstSlotPoint);


                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Open_First_Slot:
                        {
                            Point firstSlotPoint = new Point(Globals.clientRect.right - 150, 535);
                            Objects.Client.rightClickPos(firstSlotPoint);
                            System.Threading.Thread.Sleep(500);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Throw_First_Slot:
                        {
                            System.Threading.Thread.Sleep(500);
                            Point firstSlotPoint = new Point(Globals.clientRect.right - 150, 535);

                            Point sqmPosition = new Point();
                            sqmPosition.X = Objects.ClientData.GameMapCenter.X + ((waypoint.X - playerPos.X) * Objects.ClientData.SqmSize.Width);
                            sqmPosition.Y = Objects.ClientData.GameMapCenter.Y + ((waypoint.Y - playerPos.Y) * Objects.ClientData.SqmSize.Height);

                            Objects.Client.dragMouse(firstSlotPoint, sqmPosition);
                            System.Threading.Thread.Sleep(500);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Grab_Bp_From_Slot:
                        {
                            Point backpackRelativePoint = Objects.Client.equipmentPoints[(int)Equipment.Backpack];
                            Point backpackPoint = new Point(Globals.clientRect.right - Globals.clientRect.left + backpackRelativePoint.X, backpackRelativePoint.Y);

                            Point firstSlotPoint = new Point(Globals.clientRect.right - 150 , 535);
                            if (waypoint.Extra.Length > 0)
                                firstSlotPoint.X += (int.Parse(waypoint.Extra) - 1) * 38;

                            Objects.Client.dragMouse(firstSlotPoint, backpackPoint);
                            System.Threading.Thread.Sleep(500);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Throw_Bp_On_Slot:
                        {
                            Point backpackRelativePoint = Objects.Client.equipmentPoints[(int)Equipment.Backpack];
                            Point backpackPoint = new Point(Globals.clientRect.right - Globals.clientRect.left + backpackRelativePoint.X, backpackRelativePoint.Y);

                            Point firstSlotPoint = new Point(Globals.clientRect.right - 150, 535);
                            if (waypoint.Extra.Length > 0)
                                firstSlotPoint.X += (int.Parse(waypoint.Extra) - 1) * 38;

                            Objects.Client.dragMouse(backpackPoint, firstSlotPoint);
                            System.Threading.Thread.Sleep(500);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Buy_Market:
                        {
                            int itemCount = Objects.Client.getItemCount(int.Parse(extraData[1]));
                            if (itemCount < int.Parse(extraData[2]))
                                Objects.Client.buyItem(extraData[0], int.Parse(extraData[2]) - itemCount, waypoint.Position);
                            else
                                Globals.WaypointId++;

                            break;
                        }
                    case WaypointType.Sell_Market:
                        {
                            if (extraData.Length > 1)
                                Objects.Client.sellItem(extraData[0], int.Parse(extraData[1]));
                            else
                                Objects.Client.sellItem(extraData[0]);

                            System.Threading.Thread.Sleep(500);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Imbue:
                        {
                            Client.doImbue((Equipment)Enum.Parse(typeof(Equipment), extraData[0].Trim()), waypoint.Position, int.Parse(extraData[1]), int.Parse(extraData[2]));
                            System.Threading.Thread.Sleep(500);
                            Globals.WaypointId++;
                        }
                        break;
                    case WaypointType.Take_Out_Equip:
                        {
                            Client.takeOut((Equipment)Enum.Parse(typeof(Equipment), waypoint.Extra.Trim()), waypoint.Position);
                            //System.Threading.Thread.Sleep(500);
                            Globals.WaypointId++;
                        }
                        break;
                    case WaypointType.Take_In_Equip:
                        {
                            Client.takeIn((Equipment)Enum.Parse(typeof(Equipment), waypoint.Extra.Trim()), waypoint.Position);
                            //System.Threading.Thread.Sleep(500);
                            Globals.WaypointId++;
                        }
                        break;
                    case WaypointType.Setup_Bag:
                        {
                            Point middleScreenPoint = new Point((Globals.clientRect.right - Globals.clientRect.left) / 2, (Globals.clientRect.bottom - Globals.clientRect.top) / 2);

                            Point assignPoint = new Point(middleScreenPoint.X - 100, middleScreenPoint.Y - 150);
                            Point backpackFirstItemPoint = new Point(Globals.clientRect.right - 145, 535);
                            Point closeWindow = new Point(Globals.clientRect.right - 8, 510);
                            Point backpackRelativePoint = Objects.Client.equipmentPoints[(int)Equipment.Backpack];
                            Point backpackPoint = new Point(Globals.clientRect.right - Globals.clientRect.left + backpackRelativePoint.X, backpackRelativePoint.Y);

                            // OPEN CONTAINERS WINDOW
                            Keyboard.PressKey(Keys.OemMinus);
                            System.Threading.Thread.Sleep(500);

                            // CLICK ASSIGN
                            Client.leftClick(assignPoint);
                            System.Threading.Thread.Sleep(500);

                            // SET TO MAIN BACKPACK
                            Client.leftClick(backpackPoint);
                            System.Threading.Thread.Sleep(500);
                            Keyboard.PressKey(Keys.Enter);
                            System.Threading.Thread.Sleep(500);

                            Keyboard.PressKey(Keys.Escape);
                            Globals.WaypointId++;
                        }
                        break;
                    case WaypointType.Buy_AutoLoot:
                        {
                            WinApi.RECT clientRect = Globals.clientRect;
                            Point middleScreenPoint = new Point((Globals.clientRect.right - Globals.clientRect.left) / 2, (Globals.clientRect.bottom - Globals.clientRect.top) / 2);

                            Point storeButton = new Point(clientRect.right - 105, 320);
                            Point tournamentButton = new Point(middleScreenPoint.X - 275, middleScreenPoint.Y);
                            Point extrasButton = new Point(middleScreenPoint.X - 275, middleScreenPoint.Y + 82);
                            Point hadesLootButton = new Point(middleScreenPoint.X, middleScreenPoint.Y - 75);
                            Point buyButton = new Point(middleScreenPoint.X + 300, middleScreenPoint.Y - 170);


                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(500);

                            Client.leftClick(storeButton);
                            System.Threading.Thread.Sleep(500);

                            Client.leftClick(tournamentButton);
                            System.Threading.Thread.Sleep(500);

                            Client.leftClick(extrasButton);
                            System.Threading.Thread.Sleep(500);

                            Client.leftClick(hadesLootButton);
                            System.Threading.Thread.Sleep(500);

                            Client.leftClick(buyButton);
                            System.Threading.Thread.Sleep(500);

                            Keyboard.PressKey(Keys.Enter);
                            System.Threading.Thread.Sleep(500);

                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(2000);

                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(2000);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Randomize_Outfit:
                        {
                            Random rand = new Random();
                            WinApi.RECT clientRect = Globals.clientRect;

                            List<string> outfitList = new List<string> { "citizen", "hunter", "mage", "knight", "noble", "summoner", "warrior", "barbarian", "druid", "wizard", "oriental", "pirate", "shaman", "norse", "demon hunter", "warmaster", "jersey" };

                            Point middleScreenPoint = new Point((Globals.clientRect.right - Globals.clientRect.left) / 2, (Globals.clientRect.bottom - Globals.clientRect.top) / 2);

                            Point outfitNamePoint = new Point(middleScreenPoint.X + 265, middleScreenPoint.Y - 165);
                            Point firstOutfitPoint = new Point(middleScreenPoint.X + 185, middleScreenPoint.Y - 90);

                            int pLevel = Objects.Player.Level;

                            Point firstPieceOnOutfits = new Point(middleScreenPoint.X - 182, middleScreenPoint.Y + 152);
                            int pieceDist = 63;

                            Point firstColorOnOutfits = new Point(middleScreenPoint.X - 198, middleScreenPoint.Y + 169);
                            int colorDist = 14;

                            if (pLevel < 200)
                            {
                                firstPieceOnOutfits = new Point(middleScreenPoint.X - 182, middleScreenPoint.Y + 130);
                                firstColorOnOutfits = new Point(middleScreenPoint.X - 198, middleScreenPoint.Y + 149);
                            }

                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(50);
                            Keyboard.PressKey(Keys.F20);
                            System.Threading.Thread.Sleep(50);
                            Keyboard.PressKey(Keys.O);
                            System.Threading.Thread.Sleep(1000);

                            Keyboard.Write(outfitList[rand.Next(0, outfitList.Count - 1)]);
                            System.Threading.Thread.Sleep(500);

                            Client.leftClick(firstOutfitPoint);
                            System.Threading.Thread.Sleep(50);

                            for (int pieces = 0; pieces < 4; pieces++)
                            {
                                Client.leftClick(firstPieceOnOutfits.X + (pieces * pieceDist), firstPieceOnOutfits.Y);
                                System.Threading.Thread.Sleep(50);

                                Client.leftClick(firstColorOnOutfits.X + (rand.Next(0, 18) * colorDist), firstColorOnOutfits.Y + (rand.Next(0, 6) * colorDist));
                                System.Threading.Thread.Sleep(50);
                            }

                            Keyboard.PressKey(Keys.Enter);
                            System.Threading.Thread.Sleep(50);
                            Keyboard.PressKey(Keys.Escape);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Set_Offensive:
                        {
                            Keyboard.PressKey(Keys.Oemplus);
                            Globals.WaypointId++;
                        }
                        break;
                    case WaypointType.Teleport:
                        if (Math.Abs(playerPos.X - waypoint.X) > 2 || Math.Abs(playerPos.Y - waypoint.Y) > 2)
                        {
                            Globals.WaypointId++;
                            instantSkip = true;
                        }
                        else
                            Player.Goto(waypoint.X, waypoint.Y, waypoint.Z);
                        break;
                    case WaypointType.Teleport_South_East:
                        {
                            Player.Goto(playerPos.X + 1, playerPos.Y + 1, playerPos.Z);
                            System.Threading.Thread.Sleep(500);

                            Position newPos = Objects.Player.Position;
                            if (newPos.X != playerPos.X || newPos.Y != playerPos.Y || newPos.Z != playerPos.Z)
                                Globals.WaypointId++;

                            break;
                        }
                    case WaypointType.Step:
                        Player.Goto(waypoint.X, waypoint.Y, waypoint.Z);
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Wait_PZ:
                        if (Objects.Player.Creature.Skull != PlayerSkulls.White)
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Disable_Healer:
                        Globals.Main.checkBox2.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox2.Checked = false;
                        });
                        Globals.WaypointId++;
                        instantSkip = true;
                        break;
                    case WaypointType.Disable_Targeting:
                        Globals.Main.checkBox4.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox4.Checked = false;
                        });
                        Globals.WaypointId++;
                        instantSkip = true;
                        break;
                    case WaypointType.Disable_Alerts:
                        Globals.Main.checkBox5.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox5.Checked = false;
                        });
                        Globals.WaypointId++;
                        instantSkip = true;
                        break;
                    case WaypointType.Disable_Alert:
                        Globals.Main.Alerts.Invoke((MethodInvoker)delegate {
                            Globals.ScriptConfig.Alarms[(int)Enum.Parse(typeof(AlarmType), waypoint.Extra.Trim().Replace(" ", "_"), true)].CheckBox.Checked = false;
                        });
                        Globals.WaypointId++;
                        instantSkip = true;
                        break;
                    case WaypointType.Enable_Targeting:
                        Globals.Main.checkBox4.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox4.Checked = true;
                        });
                        Globals.WaypointId++;
                        instantSkip = true;
                        break;
                    case WaypointType.Enable_Alerts:
                        Globals.Main.checkBox5.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox5.Checked = true;
                        });
                        Globals.WaypointId++;
                        instantSkip = true;
                        break;
                    case WaypointType.Enable_Healer:
                        Globals.Main.checkBox2.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox2.Checked = true;
                        });
                        Globals.WaypointId++;
                        instantSkip = true;
                        break;
                    case WaypointType.Enable_Alert:
                        Globals.Main.Alerts.Invoke((MethodInvoker)delegate {
                            Globals.ScriptConfig.Alarms[(int)Enum.Parse(typeof(AlarmType), waypoint.Extra.Trim().Replace(" ", "_"), true)].CheckBox.Checked = true;
                        });
                        Globals.WaypointId++;
                        instantSkip = true;
                        break;
                    case WaypointType.Press:
                        Keyboard.PressKey((Keys)Enum.Parse(typeof(Keys), waypoint.Extra.Trim()));
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Close_Windows:
                        {
                            WinApi.RECT clientRect = Globals.clientRect;
                            Point closeWindow = new Point(clientRect.right - 8, 510);

                            for (int i = 0; i < 20; i++)
                            {
                                Client.leftClick(closeWindow.X, closeWindow.Y);
                                System.Threading.Thread.Sleep(10);
                            }

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Click_Ok:
                        {
                            WinApi.WindowPlacement placement = new WinApi.WindowPlacement();
                            WinApi.GetWindowPlacement(Globals.Process.MainWindowHandle, ref placement);

                            if (placement.showCmd == 2)
                            {
                                changedFocus = true;
                                WinApi.ShowWindow(Globals.Process.MainWindowHandle, 4);
                            }

                            Point buttonPoint = Objects.ClientData.FindOkButton();
                            if (buttonPoint.X <= 0)
                                System.Media.SystemSounds.Beep.Play();
                            else
                            {
                                Objects.Client.leftClick(buttonPoint.X, buttonPoint.Y);
                                Keyboard.PressKey(Keys.Escape);
                            }

                            Globals.WaypointId++;

                            break;
                        }
                    case WaypointType.Use_On:
                        System.Threading.Thread.Sleep(300);

                        if (Math.Abs(waypoint.X - playerPos.X) <= 7 && Math.Abs(waypoint.Y - playerPos.Y) <= 5)
                        {
                            Keyboard.PressKey((Keys)Enum.Parse(typeof(Keys), waypoint.Extra.Trim()));
                            Point sqmPosition = new Point();
                            sqmPosition.X = Objects.ClientData.GameMapCenter.X + ((waypoint.X - playerPos.X) * Objects.ClientData.SqmSize.Width);
                            sqmPosition.Y = Objects.ClientData.GameMapCenter.Y + ((waypoint.Y - playerPos.Y) * Objects.ClientData.SqmSize.Height);
                            Objects.Client.leftClick(sqmPosition.X, sqmPosition.Y);
                            System.Threading.Thread.Sleep(300);
                        }

                        Globals.WaypointId++;
                        break;
                    case WaypointType.Use:
                        System.Threading.Thread.Sleep(500);
                        if (Math.Abs(waypoint.X - playerPos.X) <= 7 && Math.Abs(waypoint.Y - playerPos.Y) <= 5)
                        {
                            Point sqmPosition = new Point();
                            sqmPosition.X = Objects.ClientData.GameMapCenter.X + ((waypoint.X - playerPos.X) * Objects.ClientData.SqmSize.Width);
                            sqmPosition.Y = Objects.ClientData.GameMapCenter.Y + ((waypoint.Y - playerPos.Y) * Objects.ClientData.SqmSize.Height);
                            Objects.Client.rightClickPos(sqmPosition.X, sqmPosition.Y);
                            System.Threading.Thread.Sleep(500);
                        }

                        Globals.WaypointId++;
                        break;
                    case WaypointType.Use_North:
                        {
                            System.Threading.Thread.Sleep(500);

                            Point sqmPosition = new Point();
                            sqmPosition.X = Objects.ClientData.GameMapCenter.X;
                            sqmPosition.Y = Objects.ClientData.GameMapCenter.Y - Objects.ClientData.SqmSize.Height;
                            Objects.Client.rightClickPos(sqmPosition.X, sqmPosition.Y);
                            System.Threading.Thread.Sleep(500);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Exit:
                        Globals.Main.Log.addLog("Exiting Character", false);
                        Threads.ClientData.UpdateCharacter();
                        Globals.Process.Kill(true);
                        Globals.Process = null;

                        Globals.WaypointId++;
                        break;
                    case WaypointType.Login_Next:
                        break;
                    case WaypointType.Logout:
                        {
                            WinApi.RECT clientRect = Globals.clientRect;

                            if (Objects.Player.Creature.Id > 0)
                                lastCharacterId = Objects.Player.Creature.Id;

                            Client.leftClick(clientRect.right - clientRect.left - 15, 345);
                            System.Threading.Thread.Sleep(500);
                            Keyboard.PressKey(Keys.Enter);
                            System.Threading.Thread.Sleep(2000);

                            if (!Objects.Player.isLoggedIn)
                                Globals.WaypointId++;

                            break;
                        }
                    case WaypointType.Minimize_Client:
                        WinApi.ShowWindow(Globals.Process.MainWindowHandle, 6);
                        Globals.WaypointId++;
                        instantSkip = true;
                        break;
                    case WaypointType.Load:
                        Classes.Script script = Program.Config.Scripts.FirstOrDefault(s => s.name.ToLower().Trim() == waypoint.Extra.Trim().ToLower());

                        if (script == null && waypoint.Extra.Trim() == string.Empty && Globals.Script != null)
                            script = Globals.Script;


                        Globals.WaypointId = 0;
                        Thread.Change(Timeout.Infinite, Timeout.Infinite);
                        Globals.ScriptConfig.GeneralStatus = false;
                        Globals.ScriptConfig.CavebotStatus = false;

                        if (script == null)
                            Globals.Load(@".\Scripts\" + @waypoint.Extra);
                        else
                            Globals.Load(script.path);

                        return;
                    case WaypointType.Alert:
                        WinApi.FlashWindow(Globals.Process.MainWindowHandle, true);
                        System.Media.SystemSounds.Beep.Play();
                        System.Threading.Thread.Sleep(3000);
                        break;
                    case WaypointType.Not_Location_Goto_Label:
                        instantSkip = true;
                        if (Math.Abs(playerPos.X - waypoint.X) < waypoint.rangeX && Math.Abs(playerPos.Y - waypoint.Y) < waypoint.rangeY && (waypoint.Z == 0 || waypoint.Z == playerPos.Z))
                            Globals.WaypointId++;
                        else
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label.Trim() == waypoint.Extra.Trim());
                        break;
                    case WaypointType.Not_Location_Goback:
                        instantSkip = true;
                        if (Math.Abs(playerPos.X - waypoint.X) < waypoint.rangeX && Math.Abs(playerPos.Y - waypoint.Y) < waypoint.rangeY && (waypoint.Z == 0 || waypoint.Z == playerPos.Z))
                            Globals.WaypointId++;
                        else if (Globals.WaypointId <= 0)
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.Count - 1;
                        else
                            Globals.WaypointId--;
                        break;
                    case WaypointType.If_Location_Goto_Label:
                        instantSkip = true;
                        if (Math.Abs(playerPos.X - waypoint.X) >= waypoint.rangeX || Math.Abs(playerPos.Y - waypoint.Y) >= waypoint.rangeY || (waypoint.Z != 0 && waypoint.Z != playerPos.Z))
                            Globals.WaypointId++;
                        else
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label.Trim() == waypoint.Extra.Trim());
                        break;
                    case WaypointType.If_Location_Goback:
                        instantSkip = true;
                        if (Math.Abs(playerPos.X - waypoint.X) >= waypoint.rangeX || Math.Abs(playerPos.Y - waypoint.Y) >= waypoint.rangeY || (waypoint.Z != 0 && waypoint.Z != playerPos.Z))
                            Globals.WaypointId++;
                        else if (Globals.WaypointId <= 0)
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.Count - 1;
                        else
                            Globals.WaypointId--;
                        break;
                    case WaypointType.Travel:
                        {
                            changedStatus = true;
                            Globals.Main.Invoke((MethodInvoker)delegate {
                                Globals.Main.checkBox2.Checked = false; // HEALER
                                Globals.Main.checkBox4.Checked = false; // TARGETING
                            });
                            Globals.ScriptConfig.auto_Haste = false;


                            isChatOn = true;
                            Keyboard.PressKey(Keys.F19);
                        System.Threading.Thread.Sleep(100);
                        Keyboard.PressKey(Keys.F22);
                        System.Threading.Thread.Sleep(500);
                        Client.Say("hi");
                        System.Threading.Thread.Sleep(500);
                        Client.Say(waypoint.Extra);
                        System.Threading.Thread.Sleep(500);
                        Client.Say("yes");
                        System.Threading.Thread.Sleep(500);
                        Keyboard.PressKey(Keys.F20);
                        System.Threading.Thread.Sleep(100);

                            break;
                        }
                    case WaypointType.Open_Npc:
                        Keyboard.PressKey(Keys.F22);
                        System.Threading.Thread.Sleep(50);
                        Globals.WaypointId++;
                        instantSkip = true;
                        break;
                    case WaypointType.If_Vocation_Goto_Label:
                        instantSkip = true;
                        if (Globals.AccVocation.ToString().ToLower() != extraData[0].Trim().ToLower())
                            Globals.WaypointId++;
                        else
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label.Trim() == extraData[1].Trim());
                        break;
                    case WaypointType.Disable_Safe:
                        instantSkip = true;
                        Threads.Alarms.safeMode = false;
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Enable_Safe:
                        instantSkip = true;
                        Threads.Alarms.safeMode = true;
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Set_Status:
                        instantSkip = true;
                        if (waypoint.Extra.Trim() != Threads.ClientData.status)
                            Globals.Main.Log.addLog(waypoint.Extra.Trim());

                        Threads.ClientData.status = waypoint.Extra.Trim();
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Set_Script:
                        instantSkip = true;
                        if (waypoint.Extra.Trim() != Globals.Script.name)
                            Threads.ClientData.UpdateScript(waypoint.Extra.Trim());

                        Globals.WaypointId++;
                        break;
                    case WaypointType.Reset_FPS:
                        {
                            if ((DateTime.Now - Threads.ClientData.lastLoginTime).TotalHours >= 6)
                            {
                                changedStatus = true;

                                Globals.Main.Invoke((MethodInvoker)delegate {
                                    Globals.Main.checkBox2.Checked = false; // HEALER
                                    Globals.Main.checkBox4.Checked = false; // TARGETING
                                });
                                Globals.ScriptConfig.auto_Haste = false;

                                isChatOn = true;
                                Keyboard.PressKey(Keys.F19);
                                System.Threading.Thread.Sleep(100);
                                Client.Say("!fps");
                                System.Threading.Thread.Sleep(2000);
                            }

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Reset_Imbue:
                        {
                            instantSkip = true;
                            Threads.ClientData.imbueTime = -1;
                            Globals.WaypointId++;
                            break;
                        }
                    default:
                        Globals.WaypointId++;
                        break;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (changedFocus)
                    WinApi.ShowWindow(Globals.Process.MainWindowHandle, 2);

                if (changedStatus)
                {
                    Globals.Main.Invoke((MethodInvoker)delegate {
                        Globals.Main.checkBox2.Checked = healerStatus;
                        Globals.Main.checkBox4.Checked = targetStatus;
                    });
                    Globals.ScriptConfig.auto_Haste = hasteStatus;
                }
                if (isChatOn)
                {
                    Keyboard.PressKey(Keys.F20);
                }

                if (Globals.ScriptConfig.GeneralStatus && Globals.ScriptConfig.CavebotStatus)
                {
                    if (instantSkip)
                        Thread.Change(0, Timeout.Infinite);
                    else
                        Thread.Change(100, Timeout.Infinite);
                }
            }
        }
    }
}
