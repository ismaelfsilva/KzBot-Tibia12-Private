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
        public static int amountToAdd = 0;

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
                    //// SITE AREA
                    ///
                    if (!firstUpdate || Math.Round((DateTime.Now - Globals.Process.StartTime).TotalSeconds) % 600 == 0)
                    {
                        try
                        {
                            var client = new HttpClient();

                            var pairs = new List<KeyValuePair<string, string>>
    {
        new KeyValuePair<string, string>("name", Objects.Player.Creature.Name),
        new KeyValuePair<string, string>("user", Environment.UserName),
        new KeyValuePair<string, string>("script", Globals.ScriptFile.Contains(@"\") ? Globals.ScriptFile.Split(@"\").LastOrDefault() : Globals.ScriptFile),
        new KeyValuePair<string, string>("level", Objects.Player.Level.ToString()),
        new KeyValuePair<string, string>("stamina", Objects.Player.Stamina.TotalSeconds.ToString()),
        new KeyValuePair<string, string>("generated", amountToAdd.ToString()),
    };

                            if (!firstUpdate)
                                pairs.Add(new KeyValuePair<string, string>("first", "1"));

                            var postContent = new FormUrlEncodedContent(pairs);

                            client.Timeout = TimeSpan.FromSeconds(5);
                            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                            client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                            client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");

                            var response = await client.PostAsync(new Uri("https://www.kzsoft.com.br/characters.php"), postContent);
                            string content = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode && content == "1")
                            {
                                Debug.WriteLine("[{0}] {1}: {2}", DateTime.Now, "Successful Data Sent", content);
                                amountToAdd = 0;
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
                            Debug.WriteLine("[{0}] {1}", DateTime.Now, ex.Message);
                            return;
                        }
                    }

                    if (!setClient)
                    {
                        System.Threading.Thread.Sleep(2000);
                        
                        Objects.ClientData.FindPositions();
                        Objects.ClientData.FindGameMapRect();
                        Objects.Client.SetCooldownAddresses();

                        setClient = true;
                    }

                    if (Globals.Config.auto_Haste && !Globals.ComboStatus && !Objects.ClientData.isHasted && Objects.Player.Level >= 14 && Objects.Player.Mana >= 60 && Objects.Client.hasCooldown(CooldownGroup.Support))
                        Keyboard.PressKey((Keys)Properties.Settings.Default.Haste_Key);

                    if (Globals.Config.auto_Utito && Globals.ComboStatus && !Objects.ClientData.isBuffed && Objects.Player.Level >= 60 && Objects.Player.Mana >= 290 && Objects.Client.hasCooldown(CooldownGroup.Support))
                        Keyboard.PressKey((Keys)Properties.Settings.Default.Utito_Key);

                    Objects.ClientData.Update();
                }
                else
                {
                    setClient = false;
                    Threads.Alarms.safeMode = false;

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
                            else
                                break;
                        }

                        System.Threading.Thread.Sleep(2000);

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
