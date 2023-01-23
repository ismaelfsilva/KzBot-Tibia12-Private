using KzBot.Objects;
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
                    case WaypointType.Buy_Market:
                    case WaypointType.Imbue:
                    case WaypointType.Go_Near:
                    case WaypointType.Teleport:
                    case WaypointType.Take_Out_Equip:
                    case WaypointType.Step:
                        if (playerPos.Z != waypoint.Z)
                        {
                            Globals.WaypointId++;
                            return;
                        }
                        break;
                    case WaypointType.Not_Location_Goto_Label:
                    case WaypointType.Not_Location_Goback:
                        if (Math.Abs(playerPos.X - waypoint.X) >= 200 && Math.Abs(playerPos.Y - waypoint.Y) >= 200)
                        {
                            Globals.WaypointId++;
                            return;
                        }
                        break;
                    default:
                        if (!(waypoint.X == 0 && waypoint.Y == 0 && waypoint.Z == 0) && Math.Abs(playerPos.X - waypoint.X) >= waypoint.rangeX && Math.Abs(playerPos.Y - waypoint.Y) >= waypoint.rangeY)
                        {
                            Globals.WaypointId++;
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
                        if (Math.Abs(playerPos.X - waypoint.X) < waypoint.rangeX  && Math.Abs(playerPos.Y - waypoint.Y) < waypoint.rangeY || playerPos.Z != waypoint.Z)
                        {
                            Globals.WaypointId++;
                            if (waypoint.rangeX == 1 && waypoint.rangeY == 1)
                                Keyboard.PressKey(Keys.Escape);
                        }
                        else if (Math.Abs(playerPos.X - waypoint.X) > 200 || Math.Abs(playerPos.Y - waypoint.Y) > 200)
                            Globals.WaypointId++;
                        else
                        {
                            Player.Goto(waypoint.X, waypoint.Y, waypoint.Z);
                            System.Threading.Thread.Sleep(500);
                        }
                        break;
                    case WaypointType.Node:
                        if (Math.Abs(playerPos.X - waypoint.X) < waypoint.rangeX  && Math.Abs(playerPos.Y - waypoint.Y) < waypoint.rangeY || playerPos.Z != waypoint.Z)
                            Globals.WaypointId++;
                        else if (Math.Abs(playerPos.X - waypoint.X) > 200 || Math.Abs(playerPos.Y - waypoint.Y) > 200)
                            Globals.WaypointId++;
                        else
                        {
                            Player.Goto(waypoint.X + new Random().Next(waypoint.rangeX * -1 + 1, waypoint.rangeX - 1), waypoint.Y + new Random().Next(waypoint.rangeY * -1 + 1, waypoint.rangeY - 1), waypoint.Z);
                            System.Threading.Thread.Sleep(500);
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
                            System.Threading.Thread.Sleep(200);

                            Client.Say(waypoint.Extra);

                            System.Threading.Thread.Sleep(200);
                            Keyboard.PressKey(Keys.F20);

                            Globals.WaypointId++;
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
                                Globals.WaypointId++;
                            }
                            else if (Globals.ComboStatus && creatures.Count <= Globals.ScriptConfig.creature_Count_To_End_Lure - creatures.Where(c => c.HealthPc <= Globals.ScriptConfig.creatures_Left_Health_To_End_Lure).Count())
                            {
                                Globals.ComboStatus = false;
                                didWaitBecauseOfPlayerOnCombo = false;
                                Globals.WaypointId++;
                            }
                            else if (!Globals.ComboStatus)
                            {
                                Globals.WaypointId++;
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

                            for (int tries = 0; tries < 1; tries++)
                            {
                                foreach (Point p in lootPointList)
                                {
                                    Objects.Client.rightClickPos(p);
                                    System.Threading.Thread.Sleep(250);
                                }
                            }
                        }
                        else
                            Keyboard.PressKey((Keys)Properties.Settings.Default.Loot_Key);
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Goto_Label:
                        Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label.Trim() == waypoint.Extra.Trim());
                        break;
                    case WaypointType.Check_Cap:
                        if (Objects.Player.Level < Globals.ScriptConfig.min_Level_To_Check_Cap)
                            Globals.WaypointId++;
                        else if (Objects.Player.Cap < int.Parse(extraData[0]))
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == extraData[1].Trim());
                        else
                            Globals.WaypointId++;   
                        break;
                    case WaypointType.Check_Level:
                        if (Objects.Player.Level < int.Parse(extraData[0]))
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == extraData[1].Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Balance:
                        if (Threads.ClientData.lastBalance > 0 && Threads.ClientData.lastBalance < int.Parse(extraData[0]))
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == extraData[1].Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Stamina:
                        if (Objects.Player.Stamina.TotalSeconds < int.Parse(extraData[0]) * 60)
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == extraData[1].Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Safe:
                        if (Threads.Alarms.safeMode)
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == waypoint.Extra.Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_PZ:
                        if (Objects.Player.Creature.Skull != PlayerSkulls.White)
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label == waypoint.Extra.Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Imbue:
                        {
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

                            System.Threading.Thread.Sleep(500);
                            isChatOn = true;
                            Keyboard.PressKey(Keys.F19);
                            System.Threading.Thread.Sleep(500);
                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(500);

                            Keyboard.PressKey(Keys.F22);
                            System.Threading.Thread.Sleep(500);

                            Client.Say("hi");
                            System.Threading.Thread.Sleep(500);

                            WinApi.RECT clientRect = Globals.clientRect;
                            Point closeWindow = new Point(clientRect.right - 8, 510);

                            for (int i = 0; i < 10; i++)
                            {
                                Client.leftClick(closeWindow.X, closeWindow.Y);
                                System.Threading.Thread.Sleep(100);
                            }

                            System.Threading.Thread.Sleep(500);
                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(500);
                            Client.Say("trade");
                            System.Threading.Thread.Sleep(500);

                            Point tradeWindow = new Point(clientRect.right - 155, 507);

                            Client.leftClick(tradeWindow.X + 125, tradeWindow.Y + 40);
                            System.Threading.Thread.Sleep(200);

                            int itemSoldWithoutCapChange = 0;
                            int lastCap = 0;
                            while (itemSoldWithoutCapChange <= 5)
                            {
                                // CLICK FIRST
                                Client.leftClick(tradeWindow.X + 25, tradeWindow.Y + 75);
                                System.Threading.Thread.Sleep(200);

                                Client.leftClick(tradeWindow.X + 125, tradeWindow.Y + 170);
                                System.Threading.Thread.Sleep(200);

                                itemSoldWithoutCapChange++;

                                if (itemSoldWithoutCapChange == 5)
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
                            System.Threading.Thread.Sleep(500);
                            Keyboard.PressKey(Keys.F20);

                            Globals.WaypointId++;
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
                            bool changedFocus = false;

                            if (placement.showCmd == 2)
                            {
                                changedFocus = true;
                                WinApi.ShowWindow(Globals.Process.MainWindowHandle, 4);
                            }

                            WinApi.RECT clientRect = Globals.clientRect;
                            Point closeWindow = new Point(clientRect.right - 8, 510);

                            for (int i = 0; i < 10; i++)
                            {
                                Client.leftClick(closeWindow.X, closeWindow.Y);
                                System.Threading.Thread.Sleep(100);
                            }

                            Point tradeWindow = new Point(clientRect.right - 155, 507);
                            int playerLevel = Objects.Player.Level;

                            isChatOn = true;
                            Keyboard.PressKey(Keys.F19);
                            System.Threading.Thread.Sleep(1000);

                            Keyboard.PressKey(Keys.F22);
                            System.Threading.Thread.Sleep(1000);

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

                                System.Threading.Thread.Sleep(1000);
                                Objects.Client.leftClick(Objects.ClientData.GameMapRect.Left, clientRect.bottom - 23);
                                System.Threading.Thread.Sleep(1000);

                                Client.Say("hi");
                                System.Threading.Thread.Sleep(1000);

                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey)) continue;
                                Client.Say(refill.Type);
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey)) continue;
                                System.Threading.Thread.Sleep(1000);

                                // Click Buy
                                Client.leftClick(tradeWindow.X + 125, tradeWindow.Y + 20);
                                System.Threading.Thread.Sleep(500);

                                // Reset Item
                                Client.leftClick(tradeWindow.X + 140, tradeWindow.Y + 105);
                                System.Threading.Thread.Sleep(500);

                                // Search Item
                                Client.leftClick(tradeWindow.X + 30, tradeWindow.Y + 105);
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey)) continue;
                                Keyboard.Write(refill.Name);
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey)) continue;
                                System.Threading.Thread.Sleep(1000);

                                // Select Item
                                Client.leftClick(tradeWindow.X + 25, tradeWindow.Y + 75);
                                System.Threading.Thread.Sleep(500);

                                // Set Count
                                Keyboard.PressKey(Keys.Escape);
                                System.Threading.Thread.Sleep(500);
                                Client.leftClick(tradeWindow.X + 95, tradeWindow.Y + 140);
                                //System.Threading.Thread.Sleep(100);
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey)) continue;
                                Keyboard.PressKey(Keys.Delete);
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey)) continue;
                                System.Threading.Thread.Sleep(100);
                                Client.leftClick(tradeWindow.X + 95, tradeWindow.Y + 140);
                                //System.Threading.Thread.Sleep(100);
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey)) continue;
                                Keyboard.Write((refill.ToBuy - itemCount).ToString());
                                if (WinApi.GetAsyncKeyState(Keys.ControlKey) || WinApi.GetAsyncKeyState(Keys.ShiftKey)) continue;
                                System.Threading.Thread.Sleep(1000);

                                // Buy
                                Client.leftClick(tradeWindow.X + 130, tradeWindow.Y + 170);
                                System.Threading.Thread.Sleep(1000);

                                if (!Globals.ScriptConfig.GeneralStatus)
                                {
                                    if (changedFocus)
                                        WinApi.ShowWindow(Globals.Process.MainWindowHandle, 2);

                                    Keyboard.PressKey(Keys.F20);
                                    System.Threading.Thread.Sleep(100);

                                    return;
                                }
                            }

                            if (changedFocus)
                                WinApi.ShowWindow(Globals.Process.MainWindowHandle, 2);


                            Keyboard.PressKey(Keys.F20);
                            System.Threading.Thread.Sleep(100);

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
                            System.Threading.Thread.Sleep(100);
                            Keyboard.PressKey(Keys.F22);
                            System.Threading.Thread.Sleep(500);

                            Client.Say("hi");
                            System.Threading.Thread.Sleep(500);
                            Client.Say("deposit all");
                            System.Threading.Thread.Sleep(100);
                            Client.Say("yes");
                            System.Threading.Thread.Sleep(1000);

                            Keyboard.PressKey(Keys.F20);
                            System.Threading.Thread.Sleep(100);

                            Globals.Main.Log.addLog("Deposited " + totalGold + " gold coins", false);

                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Transfer:
                        Globals.Main.Log.addLog("Transfering Cash", false);
                        Globals.WaypointId++;
                        break;
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
                            System.Threading.Thread.Sleep(100);
                            Keyboard.PressKey(Keys.F22);
                            System.Threading.Thread.Sleep(500);

                            Client.Say("hi");
                            System.Threading.Thread.Sleep(500);
                            Client.Say("balance");
                            System.Threading.Thread.Sleep(2000);

                            Keyboard.PressKey(Keys.F23);
                            System.Threading.Thread.Sleep(500);
                            Keyboard.PressKey(Keys.F24);
                            System.Threading.Thread.Sleep(500);

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
                            System.Threading.Thread.Sleep(100);

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
                        }
                        break;
                    case WaypointType.Imbue:
                        {
                            Client.doImbue((Equipment)Enum.Parse(typeof(Equipment), extraData[1].Trim()), waypoint.Position, int.Parse(extraData[1]), int.Parse(extraData[2]));
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
                    case WaypointType.Setup_Bag:
                        {
                            Point middleScreenPoint = new Point((Globals.clientRect.right - Globals.clientRect.left) / 2, (Globals.clientRect.bottom - Globals.clientRect.top) / 2);

                            Point assignPoint = new Point(middleScreenPoint.X - 100, middleScreenPoint.Y - 150);
                            Point backpackFirstItemPoint = new Point(Globals.clientRect.right - 145, 535);
                            Point closeWindow = new Point(Globals.clientRect.right - 8, 510);
                            Point backpackRelativePoint = Objects.Client.equipmentPoints[(int)Equipment.Backpack];
                            Point backpackPoint = new Point(Globals.clientRect.right - Globals.clientRect.left + backpackRelativePoint.X, backpackRelativePoint.Y);

                            // CLOSE WINDOWS
                            for (int i = 0; i < 10; i++)
                            {
                                Client.leftClick(closeWindow.X, closeWindow.Y);
                                System.Threading.Thread.Sleep(100);
                            }
                            System.Threading.Thread.Sleep(1000);

                            // OPEN BACKPACK
                            Client.rightClickPos(backpackPoint);
                            System.Threading.Thread.Sleep(500);

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

                            // CLICK ASSIGN
                            Client.leftClick(assignPoint);
                            System.Threading.Thread.Sleep(500);

                            // SET TO FIRST SLOT IN BP
                            Client.leftClick(backpackFirstItemPoint);
                            System.Threading.Thread.Sleep(500);
                            Keyboard.PressKey(Keys.Enter);
                            System.Threading.Thread.Sleep(500);

                            Keyboard.PressKey(Keys.Escape);
                            Globals.WaypointId++;
                        }
                        break;
                    case WaypointType.Set_Offensive:
                        {
                            Keyboard.PressKey(Keys.Oemplus);
                            Globals.WaypointId++;
                        }
                        break;
                    case WaypointType.Teleport:
                        if (Math.Abs(playerPos.X - waypoint.X) > 2 || Math.Abs(playerPos.Y - waypoint.Y) > 2)
                            Globals.WaypointId++;
                        else
                            Player.Goto(waypoint.X, waypoint.Y, waypoint.Z);
                        break;
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
                        break;
                    case WaypointType.Disable_Targeting:
                        Globals.Main.checkBox4.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox4.Checked = false;
                        });
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Disable_Alerts:
                        Globals.Main.checkBox5.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox5.Checked = false;
                        });
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Disable_Player_On_Screen_Alert:
                        Globals.Main.Alerts.Invoke((MethodInvoker)delegate {
                            Globals.ScriptConfig.Alarms[(int)AlarmType.Player_On_Screen].CheckBox.Checked = false;
                        });
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Enable_Targeting:
                        Globals.Main.checkBox4.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox4.Checked = true;
                        });
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Enable_Alerts:
                        Globals.Main.checkBox5.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox5.Checked = true;
                        });
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Enable_Healer:
                        Globals.Main.checkBox2.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox2.Checked = true;
                        });
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Enable_Player_On_Screen_Alert:
                        Globals.Main.Alerts.Invoke((MethodInvoker)delegate {
                            Globals.ScriptConfig.Alarms[(int)AlarmType.Player_On_Screen].CheckBox.Checked = true;
                        });
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Press:
                        Keyboard.PressKey((Keys)Enum.Parse(typeof(Keys), waypoint.Extra.Trim()));
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Click_Ok:
                        {
                            WinApi.WindowPlacement placement = new WinApi.WindowPlacement();
                            WinApi.GetWindowPlacement(Globals.Process.MainWindowHandle, ref placement);
                            bool changedFocus = false;

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
                                Globals.WaypointId++;
                            }

                            if (changedFocus)
                                WinApi.ShowWindow(Globals.Process.MainWindowHandle, 2);

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
                        System.Threading.Thread.Sleep(300);
                        if (Math.Abs(waypoint.X - playerPos.X) <= 7 && Math.Abs(waypoint.Y - playerPos.Y) <= 5)
                        {
                            Point sqmPosition = new Point();
                            sqmPosition.X = Objects.ClientData.GameMapCenter.X + ((waypoint.X - playerPos.X) * Objects.ClientData.SqmSize.Width);
                            sqmPosition.Y = Objects.ClientData.GameMapCenter.Y + ((waypoint.Y - playerPos.Y) * Objects.ClientData.SqmSize.Height);
                            Objects.Client.rightClickPos(sqmPosition.X, sqmPosition.Y);
                            System.Threading.Thread.Sleep(300);
                        }

                        Globals.WaypointId++;
                        break;
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
                    case WaypointType.Load:
                        Classes.Script script = Program.Config.Scripts.FirstOrDefault(s => s.name.ToLower().Trim() == waypoint.Extra.Trim().ToLower());
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
                        if (Math.Abs(playerPos.X - waypoint.X) < waypoint.rangeX && Math.Abs(playerPos.Y - waypoint.Y) < waypoint.rangeY && (waypoint.Z == 0 || waypoint.Z == playerPos.Z))
                            Globals.WaypointId++;
                        else
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label.Trim() == waypoint.Extra.Trim());
                        break;
                    case WaypointType.Not_Location_Goback:
                        if (Math.Abs(playerPos.X - waypoint.X) < waypoint.rangeX && Math.Abs(playerPos.Y - waypoint.Y) < waypoint.rangeY && (waypoint.Z == 0 || waypoint.Z == playerPos.Z))
                            Globals.WaypointId++;
                        else if (Globals.WaypointId <= 0)
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.Count - 1;
                        else
                            Globals.WaypointId--;
                        break;
                    case WaypointType.If_Location_Goto_Label:
                        if (Math.Abs(playerPos.X - waypoint.X) >= waypoint.rangeX || Math.Abs(playerPos.Y - waypoint.Y) >= waypoint.rangeY || (waypoint.Z != 0 && waypoint.Z != playerPos.Z))
                            Globals.WaypointId++;
                        else
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label.Trim() == waypoint.Extra.Trim());
                        break;
                    case WaypointType.If_Location_Goback:
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
                        System.Threading.Thread.Sleep(1000);
                        Globals.WaypointId++;
                        break;
                    case WaypointType.If_Vocation_Goto_Label:
                        if (Globals.AccVocation.ToString().ToLower() != extraData[0].Trim().ToLower())
                            Globals.WaypointId++;
                        else
                            Globals.WaypointId = Globals.ScriptConfig.Waypoints.FindIndex(w => w.Label.Trim() == extraData[1].Trim());
                        break;
                    case WaypointType.Disable_Safe:
                        Threads.Alarms.safeMode = false;
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Set_Status:
                        if (waypoint.Extra.Trim() != Threads.ClientData.status)
                            Globals.Main.Log.addLog(waypoint.Extra.Trim());

                        Threads.ClientData.status = waypoint.Extra.Trim();
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
                    Thread.Change(100, Timeout.Infinite);
                }
            }
        }
    }
}
