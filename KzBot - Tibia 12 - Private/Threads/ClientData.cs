using KzBot.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KzBot.Threads
{
    public static class ClientData
    {
        public static System.Threading.Timer Thread = new System.Threading.Timer(ClientDataThread, null, Timeout.Infinite, Timeout.Infinite);
        public static bool setClient = false;
        public static bool firstUpdate = false;
        public static int lastBalance = 0;
        public static string status = "None";
        public static int imbueTime = -1;
        public static string lastUpdatedCharacter = string.Empty;

        public static bool botIsHidden = false;
        public static Size sizeWhenVisible = new Size();
        public static bool lockSize = false;

        public static DateTime lastLoginTime = DateTime.Now;

        public static DateTime lastReconectStart = DateTime.Now;
        public static bool isReconnecting = false;
        public static int totalReconnect10Minutes = 1;

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

                if (Globals.Process == null)
                {
                    Globals.Main.Invoke((MethodInvoker)delegate
                    {
                        Globals.Main.Visible = true;
                        Globals.Main.ShowInTaskbar = true;
                    });
                }
                else if (Globals.Process.HasExited)
                {
                    Threads.ClientData.UpdateCharacter();
                    Globals.Main.Invoke((MethodInvoker)delegate
                    {
                        Globals.Main.ShowInTaskbar = true;
                        Globals.Main.canCloseForm = true;
                        Globals.Main.Close();
                        Application.Exit();
                    });
                    return;
                }
                else if (foregroundWindow == Globals.Process?.MainWindowHandle || foregroundWindow == mainHandle || Globals.Main.OwnedForms.Any(f=> f.Handle == foregroundWindow))
                {                    
                    Globals.Main.Invoke((MethodInvoker)delegate
                    {
                        if (!Globals.Main.TopMost)
                        {
                            Globals.Main.TopMost = true;
                            Globals.Main.ShowInTaskbar = false;
                        }

                        if (!botIsHidden)
                            Globals.Main.Show();

                        if (foregroundWindow == mainHandle && lockSize)
                        {
                            Globals.Main.Size = sizeWhenVisible;
                            lockSize = false;
                        }
                        else if (foregroundWindow != mainHandle && Globals.Main.Size != Globals.Main.MinimumSize)
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
                    isReconnecting = false;

                    if (Globals.ScriptConfig.auto_Haste && !Globals.ComboStatus && (playerCreature.Speed < Math.Floor((pLevel + 109) * 1.2)) && pLevel >= 14 && Objects.Player.Mana >= 60 && Objects.Client.hasCooldown(CooldownGroup.Support) && DateTime.Now > Threads.Cavebot.idleUntil)
                        Keyboard.PressKey((Keys)Properties.Settings.Default.Haste_Key);
                }
                else if (Globals.ScriptConfig.auto_Reconnect)
                {
                    if (!isReconnecting)
                    {
                        lastReconectStart = DateTime.Now;
                        totalReconnect10Minutes = 1;
                        isReconnecting = true;
                        Globals.Main.Log.addLog("Character Offline", false);
                    }
                    else if (totalReconnect10Minutes > 3)
                    {
                        if (await BanCharacter())
                        {
                            Globals.Main.Log.addLog("Banned", false);
                            Threads.ClientData.UpdateCharacter();
                            Globals.Main.Invoke((MethodInvoker)delegate
                            {
                                Globals.Main.canCloseForm = true;
                                Globals.Main.Close();
                            });
                            return;
                        }
                    }
                    else if ((DateTime.Now - lastReconectStart).TotalMinutes >= totalReconnect10Minutes * 10)
                    {
                        totalReconnect10Minutes++;

                        if (await getOnlinePlayers() < 50)
                        {
                            totalReconnect10Minutes = 1;
                            lastReconectStart = DateTime.Now;
                        }
                    }

                    setClient = false;
                    Threads.Alarms.safeMode = false;

                    Keyboard.PressKey(Keys.Escape);
                    System.Threading.Thread.Sleep(10);

                    WinApi.RECT clientRect = Globals.clientRect;

                    // CLICK OK ON SS [TEST]
                    for (int i = 0; i < 10; i++)
                    {
                        Objects.Client.leftClick((clientRect.right - clientRect.left) / 2 + 130, (clientRect.bottom - clientRect.top) / 2 + (i * 10));
                        System.Threading.Thread.Sleep(10);
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        Keyboard.PressKey(Keys.Escape);
                        System.Threading.Thread.Sleep(10);
                    }

                    Objects.Client.leftClick((clientRect.right - clientRect.left) / 2, (clientRect.bottom - clientRect.top) / 2 - 60);
                    Keyboard.Write(Globals.AccName);
                    Objects.Client.leftClick((clientRect.right - clientRect.left) / 2, (clientRect.bottom - clientRect.top) / 2 - 30);
                    Keyboard.Write(Globals.AccPass);

                    Keyboard.PressKey(Keys.Enter);
                    System.Threading.Thread.Sleep(500);

                    for (int i = 0; i < 30; i++)
                    {
                        Keyboard.PressKey(Keys.Up);
                        System.Threading.Thread.Sleep(10);
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Keyboard.PressKey(Keys.Down);
                        System.Threading.Thread.Sleep(10);
                    }

                    Keyboard.PressKey(Keys.Enter);
                    int gameServerLoginChecks = 20;
                    for (int i = 1; i <= gameServerLoginChecks; i++)
                    {
                        if (!Objects.Player.isLoggedIn)
                            System.Threading.Thread.Sleep(100);
                        else if (i >= gameServerLoginChecks - 1)
                            Keyboard.PressKey(Keys.Escape);
                        else if (Objects.Player.isLoggedIn || !Globals.ScriptConfig.GeneralStatus)
                        {
                            Globals.Main.Log.addLog("Character Online", false);
                            isReconnecting = false;
                            lastLoginTime = DateTime.Now;
                            Keyboard.PressKey(Keys.F20);
                            return;
                        }
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
        public static async Task<bool> BanCharacter()
        {
            try
            {
                var client = new HttpClient();

                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");

                var response = await client.GetAsync(new Uri(string.Format("https://tibia.kzsoft.com.br/status.php?username={0}&password={1}&char_name={2}&status={3}",
                    Globals.Username,
                    Globals.Password,
                    Globals.AccCharName,
                    -1
                    )));
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && int.Parse(content) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
            }

            return false;
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
                    imbueTime
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
                Debug.WriteLine("[{0}] {1}", DateTime.Now, ex.Message);
                return;
            }
        }
        public static async void LogMessage(string message)
        {
            try
            {
                var client = new HttpClient();

                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");

                var response = await client.GetAsync(new Uri(string.Format("https://tibia.kzsoft.com.br/log.php?username={0}&password={1}&char_name={2}&message={3}",
                    Globals.Username,
                    Globals.Password,
                    Globals.AccCharName,
                    HttpUtility.UrlEncode(message).Replace("+", "%20")
                    )));
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && content == "1")
                    Debug.WriteLine("[{0}] {1}: {2}", DateTime.Now, "Successful Data Sent", content);
                else
                    Debug.WriteLine("[{0}] {1}: {2}", DateTime.Now, "Failed Data Sent", content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[{0}] {1}", DateTime.Now, ex.Message);
                return;
            }
        }

        class serverInfo
        {
            public int gamingyoutubestreams { get; set; } = 0;
            public int gamingyoutubeviewer { get; set; } = 0;
            public int playersonline { get; set; } = 0;
            public int twitchstreams { get; set; } = 0;
            public int twitchviewer { get; set; } = 0;
        }

        public static async Task<int> getOnlinePlayers()
        {
            int onlinePlayers = 0;

            try
            {
                var client = new HttpClient();

                var postContent = JsonConvert.SerializeObject(new
                {
                    type = "cacheinfo"
                });
                var payload = new StringContent(postContent, Encoding.UTF8, "application/json");

                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");

                var response = await client.PostAsync(@Globals.Server.loginServer, payload);
                string content = await response.Content.ReadAsStringAsync();
                serverInfo serverInfo = JsonConvert.DeserializeObject<serverInfo>(content);

                if (response.IsSuccessStatusCode)
                {
                    onlinePlayers = serverInfo.playersonline;
                }
            }
            catch
            {

            }

            return onlinePlayers;
        }
    }
}
