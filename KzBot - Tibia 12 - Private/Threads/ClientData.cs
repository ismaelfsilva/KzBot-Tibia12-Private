using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzBot.Threads
{
    public static class ClientData
    {
        public static System.Threading.Timer Thread = new System.Threading.Timer(ClientDataThread, null, Timeout.Infinite, Timeout.Infinite);
        public static bool setClient = false;

        private static void ClientDataThread(object? state)
        {
            Thread.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                IntPtr foregroundWindow = WinApi.GetForegroundWindow();
                IntPtr mainHandle = (IntPtr) 0x0;

                Globals.Main.Invoke((MethodInvoker)delegate {
                    mainHandle = Globals.Main.Handle;
                });

                if ((foregroundWindow == Globals.Process?.MainWindowHandle || foregroundWindow == mainHandle) && WinApi.GetAsyncKeyState(Keys.Pause))
                {
                    Globals.Main.checkBox1.Invoke((MethodInvoker)delegate {
                        Globals.Main.checkBox1.Checked = !Globals.Main.checkBox1.Checked;
                    });

                    System.Media.SystemSounds.Beep.Play();

                    if (!Globals.Config.GeneralStatus)
                    {
                        System.Threading.Thread.Sleep(1000);
                        return;
                    }
                }

                if (!Globals.Config.GeneralStatus || Globals.Process == null || Globals.Process.HasExited)
                    return;


                if (Objects.Player.isLoggedIn)
                {
                    if (!setClient)
                    {
                        System.Threading.Thread.Sleep(2000);
                        
                        Objects.ClientData.FindPositions();
                        Objects.ClientData.FindGameMapRect();
                        Objects.Client.SetCooldownAddresses();

                        setClient = true;
                    }

                    int playerLevel = Objects.Player.Level;

                    if (Globals.Config.auto_Haste && !Globals.ComboStatus && !Objects.ClientData.isHasted && playerLevel >= 14 && Objects.Client.hasCooldown(CooldownGroup.Support))
                        Keyboard.PressKey((Keys)Properties.Settings.Default.Haste_Key);

                    if (Globals.Config.auto_Utito && Globals.ComboStatus && !Objects.ClientData.isBuffed && playerLevel >= 60 && Objects.Client.hasCooldown(CooldownGroup.Support))
                        Keyboard.PressKey((Keys)Properties.Settings.Default.Utito_Key);

                    Objects.ClientData.UpdateStatus();
                    Objects.ClientData.Update();
                }
                else
                {
                    setClient = false;

                    System.Threading.Thread.Sleep(5000);

                    if (Globals.AccountId != -1)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(100);
                        }

                        Accounts.Account account = Globals.Accounts.List[Globals.AccountId];

                        Keyboard.Write(account.AccountName);
                        System.Threading.Thread.Sleep(100);
                        Keyboard.PressKey(Keys.Tab);
                        System.Threading.Thread.Sleep(100);
                        Keyboard.Write(account.Password);
                        System.Threading.Thread.Sleep(100);
                        Keyboard.PressKey(Keys.Enter);
                        System.Threading.Thread.Sleep(5000);

                        for (int i = 0; i < 30; i++)
                        {
                            Keyboard.PressKey(Keys.Up);
                            System.Threading.Thread.Sleep(20);
                        }

                        for (int i = 0; i < account.Index - 1; i++)
                        {
                            Keyboard.PressKey(Keys.Down);
                            System.Threading.Thread.Sleep(50);
                        }

                        Keyboard.PressKey(Keys.Enter);
                        for (int i = 0; i < 50; i++)
                        {
                            if (!Objects.Player.isLoggedIn)
                                System.Threading.Thread.Sleep(100);
                        }

                        Point buttonPoint = Objects.ClientData.FindOkButton();
                        if (buttonPoint.X > 0)
                        {
                            Objects.Client.leftClick(buttonPoint.X, buttonPoint.Y);
                            Keyboard.PressKey(Keys.Escape);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
        
            }
            finally
            {
                Thread.Change(Globals.Config.client_Data_Update_Rate, Timeout.Infinite);
            }
        }
    }
}
