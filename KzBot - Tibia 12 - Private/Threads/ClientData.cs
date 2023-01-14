using KzBot.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzBot.Threads
{
    public static class ClientData
    {
        public static System.Threading.Timer Thread = new System.Threading.Timer(ClientDataThread, null, Timeout.Infinite, Timeout.Infinite);
        public static bool setClient = false;
        public static bool firstUpdate = false;
        public static int lastBalance = 0;
        public static string status = "init";
        public static int hasImbuement = -1;
        public static string lastUpdatedCharacter = string.Empty;

        public static bool botIsHidden = false;
        public static Size sizeWhenVisible = new Size();
        public static bool lockSize = false;

        private async static void ClientDataThread(object? state)
        {
            Thread.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                IntPtr foregroundWindow = WinApi.GetForegroundWindow();
                IntPtr mainHandle = (IntPtr) 0x0;

                Globals.Main.Invoke((MethodInvoker)delegate {
                    mainHandle = Globals.Main.Handle;
                });

                if (Globals.Process == null || Globals.Process.HasExited)
                {
                    Globals.Main.Invoke((MethodInvoker)delegate
                    {
                        Globals.Main.Visible = true;
                        Globals.Main.ShowInTaskbar = true;
                    });
                }
                else if (foregroundWindow == Globals.Process?.MainWindowHandle || foregroundWindow == mainHandle || Globals.Main.OwnedForms.Count(f=> f.Handle == foregroundWindow) > 0)
                {
                    Globals.Main.Invoke((MethodInvoker)delegate
                    {
                        Globals.Main.TopMost = true;
                        Globals.Main.ShowInTaskbar = false;

                        if (!botIsHidden)
                            Globals.Main.Show();

                        if (foregroundWindow == mainHandle && lockSize)
                        {
                            Globals.Main.Size = sizeWhenVisible;
                            lockSize = false;
                        }
                        else if (foregroundWindow == Globals.Process?.MainWindowHandle && Globals.Main.Size != Globals.Main.MinimumSize)
                        {
                            lockSize = true;
                            sizeWhenVisible = Globals.Main.Size;
                            Globals.Main.Size = Globals.Main.MinimumSize;
                        }
                    });
                    if (WinApi.GetAsyncKeyState(Keys.Pause))
                    {
                        Globals.Main.checkBox1.Invoke((MethodInvoker)delegate {
                            Globals.Main.checkBox1.Checked = !Globals.Main.checkBox1.Checked;
                        });

                        System.Media.SystemSounds.Beep.Play();

                        if (!Globals.ScriptConfig.GeneralStatus)
                        {
                            System.Threading.Thread.Sleep(1000);
                            return;
                        }
                    }
                    else if (WinApi.GetAsyncKeyState(Keys.ControlKey) && WinApi.GetAsyncKeyState(Keys.Home))
                    {
                        Globals.Main.Invoke((MethodInvoker)delegate {
                            Globals.Main.Visible = !Globals.Main.Visible;
                            botIsHidden = !Globals.Main.Visible;
                            if (Globals.Main.Visible)
                                WinApi.SetForegroundWindow(Globals.Main.Handle);
                        });
                    }
                }
                else
                {
                    Globals.Main.Invoke((MethodInvoker)delegate
                    {
                        Globals.Main.Hide();
                        Globals.Main.TopMost = false;
                        Globals.Main.ShowInTaskbar = false;
                    });
                }
                if (Globals.Process == null || Globals.Process.HasExited)
                    return;

                // UPDATE CLIENT RECT

                WinApi.RECT cRect;
                WinApi.GetClientRect(Globals.Process.MainWindowHandle, out cRect);

                if (Globals.clientRect.bottom <= 0 || (cRect.bottom > 0 && cRect.bottom != Globals.clientRect.bottom))
                {
                    WinApi.WindowPlacement placement = new WinApi.WindowPlacement();
                    WinApi.GetWindowPlacement(Globals.Process.MainWindowHandle, ref placement);
                    bool changedFocus = false;

                    if (placement.showCmd == 2)
                    {
                        changedFocus = true;
                        WinApi.ShowWindow(Globals.Process.MainWindowHandle, 4);
                    }

                    if (cRect.right > 0 && cRect.bottom > 0)
                        Globals.clientRect = cRect;
                    else
                        return;

                    if (changedFocus)
                        WinApi.ShowWindow(Globals.Process.MainWindowHandle, 2);
                }


                if (!Globals.ScriptConfig.GeneralStatus)
                    return;

                if (Objects.Player.isLoggedIn)
                {
                    Creature playerCreature = Objects.Player.Creature;

                    if (!firstUpdate || (playerCreature.Name != string.Empty && lastUpdatedCharacter != playerCreature.Name) || Math.Round((DateTime.Now - Globals.Process.StartTime).TotalSeconds) % 60 == 0)
                    {
                        lastUpdatedCharacter = playerCreature.Name;
                        UpdateCharacter();
                        System.Threading.Thread.Sleep(1100);
                    }

                    if (!setClient)
                    {
                        System.Threading.Thread.Sleep(2000);

                        //Objects.ClientData.FindPositions();
                        Objects.ClientData.FindGameMapRect();
                        Objects.Client.SetCooldownAddresses();

                        Globals.HasAutoLoot = Globals.Server.autoLootId != -1 && Objects.Client.getItemCount(Globals.Server.autoLootId) > 0;

                        setClient = true;
                    }

                    int pLevel = Objects.Player.Level;

                    if (Globals.ScriptConfig.auto_Haste && !Globals.ComboStatus && (playerCreature.Speed < Math.Floor((pLevel + 109) * 1.2)) && pLevel >= 14 && Objects.Player.Mana >= 60 && Objects.Client.hasCooldown(CooldownGroup.Support))
                        Keyboard.PressKey((Keys)Properties.Settings.Default.Haste_Key);
                }
                else if (Globals.ScriptConfig.auto_Reconnect)
                {
                    setClient = false;
                    Threads.Alarms.safeMode = false;

                    System.Threading.Thread.Sleep(1000);
                        for (int i = 0; i < 10; i++)
                        {
                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(100);

                            if (Objects.Player.isLoggedIn || !Globals.ScriptConfig.GeneralStatus)
                                return;
                        }



                        if (Objects.Player.isLoggedIn || !Globals.ScriptConfig.GeneralStatus)
                            return;

                        WinApi.RECT clientRect = Globals.clientRect;

                        // CLICK OK ON SS [TEST]
                        for (int i = 0; i < 10; i++)
                        {
                            Objects.Client.leftClick((clientRect.right - clientRect.left) / 2 + 130, (clientRect.bottom - clientRect.top) / 2 + (i * 10));
                            System.Threading.Thread.Sleep(100);

                            if (Objects.Player.isLoggedIn || !Globals.ScriptConfig.GeneralStatus)
                                return;
                        }

                        for (int i = 0; i < 10; i++)
                        {
                            Keyboard.PressKey(Keys.Escape);
                            System.Threading.Thread.Sleep(100);

                            if (Objects.Player.isLoggedIn || !Globals.ScriptConfig.GeneralStatus)
                                return;
                        }

                        if (Objects.Player.isLoggedIn || !Globals.ScriptConfig.GeneralStatus)
                            return;


                        Objects.Client.leftClick((clientRect.right - clientRect.left) / 2, (clientRect.bottom - clientRect.top) / 2 - 60);
                        System.Threading.Thread.Sleep(100);
                        Keyboard.Write(Globals.AccName);
                        System.Threading.Thread.Sleep(100);

                        if (Objects.Player.isLoggedIn || !Globals.ScriptConfig.GeneralStatus)
                            return;

                        Objects.Client.leftClick((clientRect.right - clientRect.left) / 2, (clientRect.bottom - clientRect.top) / 2 - 30);
                        System.Threading.Thread.Sleep(100);
                        Keyboard.Write(Globals.AccPass);
                        System.Threading.Thread.Sleep(100);

                        if (Objects.Player.isLoggedIn || !Globals.ScriptConfig.GeneralStatus)
                            return;

                        Keyboard.PressKey(Keys.Enter);
                        System.Threading.Thread.Sleep(5000);

                        if (Objects.Player.isLoggedIn || !Globals.ScriptConfig.GeneralStatus)
                            return;

                        for (int i = 0; i < 30; i++)
                        {
                            Keyboard.PressKey(Keys.Up);
                            System.Threading.Thread.Sleep(50);

                            if (Objects.Player.isLoggedIn || !Globals.ScriptConfig.GeneralStatus)
                                return;
                        }

                        for (int i = 0; i < Globals.AccCharIndex; i++)
                        {
                            Keyboard.PressKey(Keys.Down);
                            System.Threading.Thread.Sleep(100);

                            if (Objects.Player.isLoggedIn || !Globals.ScriptConfig.GeneralStatus)
                                return;
                        }

                        Keyboard.PressKey(Keys.Enter);
                        for (int i = 0; i < 50; i++)
                        {
                            if (!Objects.Player.isLoggedIn)
                                System.Threading.Thread.Sleep(100);
                            else if (Objects.Player.isLoggedIn || !Globals.ScriptConfig.GeneralStatus)
                                return;
                            else
                                break;
                        }
                    }
            }
            catch (Exception ex)
            {
        
            }
            finally
            {
                Thread.Change(100, Timeout.Infinite);
            }
        }

        public static async void UpdateCharacter()
        {
            try
            {
                var client = new HttpClient();

                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");

                var response = await client.GetAsync(new Uri(string.Format("https://tibia.kzsoft.com.br/character.php?username={0}&password={1}&char_name={2}&char_level={3}&char_balance={4}&char_stamina={5}&script_status={6}&imbue={7}",
                    Globals.Username,
                    Globals.Password,
                    Globals.AccCharName,
                    Objects.Player.Level,
                    lastBalance,
                    Objects.Player.Stamina.TotalSeconds,
                    status == string.Empty ? "None" : status,
                    Convert.ToInt32(hasImbuement)
                    )));
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && content == "1")
                {
                    Debug.WriteLine("[{0}] {1}: {2}", DateTime.Now, "Successful Data Sent", content);
                    firstUpdate = true;
                }
                else
                {
                    Debug.WriteLine("[{0}] {1}: {2}", DateTime.Now, "Failed Data Sent", content);
                }

                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Debug.WriteLine("[{0}] {1}", DateTime.Now, ex.Message);
                return;
            }
        }
    }
}
