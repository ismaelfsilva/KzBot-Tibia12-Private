using KzBot.Objects;
using KzBot.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace KzBot.Threads
{
    public static class Cavebot
    {
        public static System.Threading.Timer Thread = new System.Threading.Timer(CavebotThread, null, Timeout.Infinite, Timeout.Infinite);
        public static bool didWaitBecauseOfPlayerOnCombo = false;
        private static void CavebotThread(object state)
        {
            Thread.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                if (Globals.Config.Waypoints.Count <= 0)
                    return;

                if (Globals.Config.Waypoints.Count > 0 && Globals.WaypointId >= Globals.Config.Waypoints.Count)
                    Globals.WaypointId = 0;

                Waypoint waypoint = Globals.Config.Waypoints[Globals.WaypointId];

                if (waypoint.Type == WaypointType.Login_Next)
                {

                    if (!Objects.Player.isLoggedIn || (waypoint.X == 0 && waypoint.Y == 0 && waypoint.Z == 0) || (Math.Abs(Objects.Player.Position.X - waypoint.X) < waypoint.rangeX && Math.Abs(Objects.Player.Position.Y - waypoint.Y) < waypoint.rangeY && (waypoint.Z == 0 || waypoint.Z == Objects.Player.Position.Z)))
                    {
                        Globals.Process?.Kill();

                        new System.Threading.Thread(() => {
                            Globals.Accounts.List.Find(a => Globals.Accounts.List.FindIndex(acc => acc.Character == a.Character) > Globals.AccountId && a.Script == Globals.Accounts.List[Globals.AccountId].Script)?.Start();
                      }).Start();

                        Globals.WaypointId = 0;
                        Globals.Config.GeneralStatus = false;
                        Globals.Main.checkBox1.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox1.Checked = false;
                        });

                        return;
                    }
                    else
                    {
                        Globals.WaypointId++;
                        return;
                    }

                }

                if (!Globals.Config.GeneralStatus || !Globals.Config.CavebotStatus || Globals.Process == null || Globals.Process.HasExited || !Objects.Player.isLoggedIn)
                    return;


                if (Globals.Config.stop_Walking_on_Target && Objects.Player.isAttacking)
                    return;

                Position playerPos = Objects.Player.Position;

                if (waypoint == null)
                    return;

                switch (waypoint.Type)
                {
                    // WAYPOINTS THAT DON'T NEED THE PLAYER TO BE IN RANGE TO EXECUTE
                    case WaypointType.Stand:
                    case WaypointType.Node:
                    case WaypointType.Use:
                    case WaypointType.Use_On:
                    case WaypointType.Go_Near:
                    case WaypointType.Teleport:
                    case WaypointType.Step:
                        if (playerPos.Z != waypoint.Z)
                        {
                            Globals.WaypointId++;
                            return;
                        }
                        break;
                    case WaypointType.Not_Location_Goto_Label:
                    case WaypointType.Not_Location_Goback:
                    case WaypointType.If_Location_Goto_Label:
                    case WaypointType.If_Location_Goback:
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


                if (Player.isWalking && (Math.Abs(Player.GotoX - waypoint.X) < waypoint.rangeX && Math.Abs(Player.GotoY - waypoint.Y) < waypoint.rangeY && Player.GotoZ == waypoint.Z))
                    return;

                string[] extraData = waypoint.Extra.Split(";");

                switch (waypoint.Type)
                {
                    case WaypointType.Stand:
                        if (Math.Abs(playerPos.X - waypoint.X) < waypoint.rangeX  && Math.Abs(playerPos.Y - waypoint.Y) < waypoint.rangeY || playerPos.Z != waypoint.Z)
                            Globals.WaypointId++;
                        else if (Math.Abs(playerPos.X - waypoint.X) > 200 || Math.Abs(playerPos.Y - waypoint.Y) > 200)
                            Globals.WaypointId++;
                        else
                            Player.Goto(waypoint.X, waypoint.Y, waypoint.Z);
                        break;
                    case WaypointType.Node:
                        if (Math.Abs(playerPos.X - waypoint.X) < waypoint.rangeX  && Math.Abs(playerPos.Y - waypoint.Y) < waypoint.rangeY || playerPos.Z != waypoint.Z)
                            Globals.WaypointId++;
                        else if (Math.Abs(playerPos.X - waypoint.X) > 200 || Math.Abs(playerPos.Y - waypoint.Y) > 200)
                            Globals.WaypointId++;
                        else
                            Player.Goto(waypoint.X + new Random().Next(waypoint.rangeX * -1 + 1, waypoint.rangeX - 1), waypoint.Y + new Random().Next(waypoint.rangeY * -1 + 1, waypoint.rangeY - 1), waypoint.Z);
                        break;
                    case WaypointType.Say:
                        Client.Say(waypoint.Extra);
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Wait:
                        System.Threading.Thread.Sleep(int.Parse(waypoint.Extra));
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Lure:
                        {
                            List<Creature> creatures = Battlelist.getCreaturesOnScreen().FindAll(cr => cr.HealthPc > 0);
                            if (Globals.Config.check_Only_Near_Creatures_If_Player_on_Screen && creatures.Exists(c => c.Type == CreatureType.Player && c.Address != Player.Creature.Address))
                            {
                                if (!didWaitBecauseOfPlayerOnCombo)
                                {
                                    System.Threading.Thread.Sleep(Globals.Config.time_To_Wait_Before_Checking_Creatures_If_Player_on_Screen);
                                    didWaitBecauseOfPlayerOnCombo = true;
                                }

                                playerPos = Objects.Player.Position;
                                creatures.RemoveAll(c => Math.Abs(c.X - playerPos.X) > 1 || Math.Abs(c.Y - playerPos.Y) > 1);
                            }

                            creatures.RemoveAll(c => c.Type != CreatureType.Monster);

                            if (!Globals.ComboStatus && creatures.Count >= Globals.Config.creature_Count_To_Skip_Lure)
                                Globals.ComboStatus = true;
                            else if (Globals.ComboStatus && creatures.Count <= Globals.Config.creature_Count_To_End_Lure - creatures.Where(c => c.HealthPc <= Globals.Config.creatures_Left_Health_To_End_Lure).Count())
                            {
                                Globals.ComboStatus = false;
                                didWaitBecauseOfPlayerOnCombo = false;
                                Globals.WaypointId++;
                            }
                            else if (!Globals.ComboStatus)
                            {
                                Globals.WaypointId++;
                                didWaitBecauseOfPlayerOnCombo = false;
                                if (Globals.Config.Waypoints[Globals.WaypointId].Type == WaypointType.Loot)
                                    Globals.WaypointId++;
                            }

                            break;

                        }
                    case WaypointType.Loot:
                        if (Globals.Config.manual_Looting)
                        {
                            for (int tries = 0; tries < 5; tries++)
                            {
                                for (int x = -1; x <= 1; x++)
                                {
                                    for (int y = -1; y <= 1; y++)
                                    {
                                        Point sqmPosition = new Point();
                                        sqmPosition.X = Objects.ClientData.GameMapCenter.X + x * Objects.ClientData.SqmSize.Width;
                                        sqmPosition.Y = Objects.ClientData.GameMapCenter.Y + y * Objects.ClientData.SqmSize.Height;
                                        Objects.Client.rightClickPos(sqmPosition.X, sqmPosition.Y);
                                        System.Threading.Thread.Sleep(10);
                                    }
                                }
                            }
                        }
                        else
                            Keyboard.PressKey((Keys)Properties.Settings.Default.Loot_Key);
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Goto_Label:
                        Globals.WaypointId = Globals.Config.Waypoints.FindIndex(w => w.Label.Trim() == waypoint.Extra.Trim());
                        break;
                    case WaypointType.Check_Cap:
                        if (Objects.Player.Cap < int.Parse(extraData[0]))
                            Globals.WaypointId = Globals.Config.Waypoints.FindIndex(w => w.Label == extraData[1].Trim());
                        else
                            Globals.WaypointId++;   
                        break;
                    case WaypointType.Check_Level:
                        if (Objects.Player.Level < int.Parse(extraData[0]))
                            Globals.WaypointId = Globals.Config.Waypoints.FindIndex(w => w.Label == extraData[1].Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Stamina:
                        if (Objects.Player.Stamina.TotalSeconds < int.Parse(extraData[0]) * 60)
                            Globals.WaypointId = Globals.Config.Waypoints.FindIndex(w => w.Label == extraData[1].Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Safe:
                        if (Threads.Alarms.safeMode)
                            Globals.WaypointId = Globals.Config.Waypoints.FindIndex(w => w.Label == waypoint.Extra.Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Imbue:
                        if (true == false && waypoint.Extra.Trim().ToLower() == "audio")
                            using (var soundPlayer = new SoundPlayer(@"Sounds\Siren.wav"))
                            {
                                soundPlayer.Play();
                                System.Threading.Thread.Sleep(1000);
                            }
                        else if (true == false)
                            Globals.WaypointId = Globals.Config.Waypoints.FindIndex(w => w.Label == waypoint.Extra.Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Check_Refill: 
                        bool needRefill = false;
                        foreach (RefillRule refill in Globals.Config.Refill)
                        {
                            if (Objects.Client.getItemCount(refill.Id) < refill.ToLeave)
                            {
                                needRefill = true;
                                break;
                            }
                        }
                        if (needRefill)
                            Globals.WaypointId = Globals.Config.Waypoints.FindIndex(w => w.Label == waypoint.Extra.Trim());
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Go_Near:
                        Creature cr = Objects.Battlelist.getCreaturesOnScreen().Find(c => c.Name.ToLower() == waypoint.Extra.Trim().ToLower());
                        if (cr != null && cr.Position.distanceTo(playerPos) > 1)
                            Objects.Player.Goto(cr.Position);
                        else
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Sell_All:
                        {
                            System.Threading.Thread.Sleep(500);
                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(500);

                            Client.Say("#s hi");
                            System.Threading.Thread.Sleep(3000);
                            Client.Say("trade");
                            System.Threading.Thread.Sleep(500);

                            Point tradeWindow = Objects.ClientData.FindTrade();
                            Point okButton = Objects.ClientData.FindOkButton();
                            if (tradeWindow.X <= 0 || okButton.X <= 0)
                            {
                                Globals.WaypointId++;
                                return;
                            }

                            Client.leftClick(tradeWindow.X + 125, tradeWindow.Y + 40);
                            System.Threading.Thread.Sleep(50);

                            int itemSoldWithoutCapChange = 0;
                            int lastCap = 0;
                            while (itemSoldWithoutCapChange <= 10)
                            {
                                // CLICK FIRST
                                Client.leftClick(tradeWindow.X + 25, tradeWindow.Y + 75);
                                System.Threading.Thread.Sleep(50);
                                //Client.leftClick(okButton.X - 20, okButton.Y - 35);
                                //System.Threading.Thread.Sleep(300);
                                Client.leftClick(okButton.X + 5, okButton.Y + 5);
                                System.Threading.Thread.Sleep(50);

                                itemSoldWithoutCapChange++;

                                if (itemSoldWithoutCapChange == 10)
                                {
                                    int playerCap = Objects.Player.Cap;
                                    if (lastCap != playerCap)
                                        itemSoldWithoutCapChange = 0;

                                    lastCap = playerCap;
                                }

                            }
                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Buy_Refill:
                        {
                            bool saidHi = false;
                            Point tradeWindow;
                            Point okButton;
                            foreach (RefillRule refill in Globals.Config.Refill)
                            {
                                int itemCount = Objects.Client.getItemCount(refill.Id);

                                if (itemCount >= refill.ToBuy)
                                {
                                    continue;
                                }

                                System.Threading.Thread.Sleep(500);
                                Keyboard.PressKey(Keys.Escape);
                                System.Threading.Thread.Sleep(500);

                                if (!saidHi)
                                {
                                    Client.Say("#s hi");
                                    System.Threading.Thread.Sleep(3000);
                                    saidHi = true;
                                }

                                Client.Say(refill.Type);
                                System.Threading.Thread.Sleep(500);
                                tradeWindow = Objects.ClientData.FindTrade();
                                okButton = Objects.ClientData.FindOkButton();
                                if (tradeWindow.X <= 0 || okButton.X <= 0)
                                {
                                    Globals.WaypointId++;
                                    return;
                                }

                                // Click Buy
                                Client.leftClick(tradeWindow.X + 125, tradeWindow.Y + 20);
                                System.Threading.Thread.Sleep(500);

                                // Search Item
                                Client.leftClick(okButton.X - 65, okButton.Y - 50);
                                System.Threading.Thread.Sleep(500);
                                Keyboard.Write(refill.Name);
                                System.Threading.Thread.Sleep(500);

                                // Select Item
                                Client.leftClick(tradeWindow.X + 25, tradeWindow.Y + 75);
                                System.Threading.Thread.Sleep(500);

                                // Set Count
                                Client.leftClick(okButton.X - 20, okButton.Y - 20);
                                System.Threading.Thread.Sleep(500);
                                Keyboard.Write((refill.ToBuy - itemCount).ToString());
                                System.Threading.Thread.Sleep(500);

                                // Buy
                                Client.leftClick(okButton.X + 5, okButton.Y + 5);
                                System.Threading.Thread.Sleep(500);
                            }
                            Globals.WaypointId++;
                            break;
                        }
                    case WaypointType.Deposit_All:
                        System.Threading.Thread.Sleep(2000);
                        Client.Say("#s hi");
                        System.Threading.Thread.Sleep(3000);
                        Client.Say("deposit all");
                        System.Threading.Thread.Sleep(100);
                        Client.Say("yes");
                        System.Threading.Thread.Sleep(1000);

                        Globals.WaypointId++;
                        break;
                    case WaypointType.Transfer:
                        if (Globals.characterToTransfer != String.Empty)
                        {
                            Client.Say("hi");
                            System.Threading.Thread.Sleep(3000);
                            for (int i = 0; i < 5; i++)
                            {
                                Client.Say("transfer");
                                System.Threading.Thread.Sleep(100);
                                Client.Say(Globals.Config.qty_To_Transfer.ToString());
                                System.Threading.Thread.Sleep(100);
                                Client.Say(Globals.characterToTransfer);
                                System.Threading.Thread.Sleep(100);
                                Client.Say("yes");
                                System.Threading.Thread.Sleep(100);
                            }
                            System.Threading.Thread.Sleep(1000);
                        }
                        Globals.WaypointId++;
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
                        if (!Objects.ClientData.isPzLocked)
                            Globals.WaypointId++;
                        break;
                    case WaypointType.Disable_Targeting:
                        Globals.Main.checkBox4.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox4.Checked = false;
                        });
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Disable_Alerts:
                        Globals.Main.checkBox4.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox5.Checked = false;
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
                    case WaypointType.Press:
                        Keyboard.PressKey((Keys)Enum.Parse(typeof(Keys), waypoint.Extra.Trim()));
                        Globals.WaypointId++;
                        break;
                    case WaypointType.Click_Ok:
                        Point buttonPoint = Objects.ClientData.FindOkButton();
                        if (buttonPoint.X <= 0)
                            System.Media.SystemSounds.Beep.Play();
                        else
                        {
                            Objects.Client.leftClick(buttonPoint.X, buttonPoint.Y);
                            Keyboard.PressKey(Keys.Escape);
                            Globals.WaypointId++;
                        }

                        break;
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
                        Globals.Process.Kill(true);
                        Globals.Process = null;
                        break;
                    case WaypointType.Login_Next:
                        break;
                    case WaypointType.Load:
                        Globals.Load(@".\Scripts\" + @waypoint.Extra);
                        Globals.WaypointId = 0;
                        System.Threading.Thread.Sleep(5000);
                        break;
                    case WaypointType.Alert:
                        WinApi.FlashWindow(Globals.Process.MainWindowHandle, true);
                        System.Media.SystemSounds.Beep.Play();
                        System.Threading.Thread.Sleep(3000);
                        break;
                    case WaypointType.Not_Location_Goto_Label:
                        if (Math.Abs(playerPos.X - waypoint.X) < waypoint.rangeX && Math.Abs(playerPos.Y - waypoint.Y) < waypoint.rangeY && (waypoint.Z == 0 || waypoint.Z == playerPos.Z))
                            Globals.WaypointId++;
                        else
                            Globals.WaypointId = Globals.Config.Waypoints.FindIndex(w => w.Label.Trim() == waypoint.Extra.Trim());
                        break;
                    case WaypointType.Not_Location_Goback:
                        if (Math.Abs(playerPos.X - waypoint.X) < waypoint.rangeX && Math.Abs(playerPos.Y - waypoint.Y) < waypoint.rangeY && (waypoint.Z == 0 || waypoint.Z == playerPos.Z))
                            Globals.WaypointId++;
                        else if (Globals.WaypointId <= 0)
                            Globals.WaypointId = Globals.Config.Waypoints.Count - 1;
                        else
                            Globals.WaypointId--;
                        break;
                    case WaypointType.If_Location_Goto_Label:
                        if (Math.Abs(playerPos.X - waypoint.X) >= waypoint.rangeX || Math.Abs(playerPos.Y - waypoint.Y) >= waypoint.rangeY || (waypoint.Z != 0 && waypoint.Z != playerPos.Z))
                            Globals.WaypointId++;
                        else
                            Globals.WaypointId = Globals.Config.Waypoints.FindIndex(w => w.Label.Trim() == waypoint.Extra.Trim());
                        break;
                    case WaypointType.If_Location_Goback:
                        if (Math.Abs(playerPos.X - waypoint.X) >= waypoint.rangeX || Math.Abs(playerPos.Y - waypoint.Y) >= waypoint.rangeY || (waypoint.Z != 0 && waypoint.Z != playerPos.Z))
                            Globals.WaypointId++;
                        else if (Globals.WaypointId <= 0)
                            Globals.WaypointId = Globals.Config.Waypoints.Count - 1;
                        else
                            Globals.WaypointId--;
                        break;
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
                if (Globals.Config.GeneralStatus && Globals.Config.CavebotStatus)
                    Thread.Change(100, Timeout.Infinite);
            }
        }
    }
}
