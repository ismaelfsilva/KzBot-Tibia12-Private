using KzBot.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace KzBot.Threads
{
    public static class Alarms
    {
        public static System.Threading.Timer Thread = new System.Threading.Timer(AlarmThread, null, Timeout.Infinite, Timeout.Infinite);
        private static DateTime lastScreenshot = DateTime.MinValue;
        private static DateTime lastTarget = DateTime.MinValue;

        private static List<string> gmNames = new List<string>() { "Lucas", "worthdavi", "Shivas"};
        private static int ticksStuck = 0;
        private static Position lastCheckPosition = new Position() { X = 0, Y = 0, Z = 0 };

        private static DateTime lastTelegramMessage = DateTime.MinValue;

        public static bool safeMode = false;
        public static bool hasImbue = true;

        public static string extraInfo = string.Empty;

        public static DateTime lastCheckImbueTime = DateTime.MinValue;

        public static string lastStatusMessage = string.Empty;
        public static string lastAlarmStatusMessage = string.Empty;

        private static async void AlarmThread(object? state)
        {
            Thread.Change(Timeout.Infinite, Timeout.Infinite);

            bool changedStatus = false;

            try
            {
                if (!Globals.ScriptConfig.GeneralStatus)
                    return;

                List<AlarmType> alarmsRequested = new List<AlarmType>();

                if (Globals.Process == null || Globals.Process.HasExited || !Objects.Player.isLoggedIn || !Objects.Player.isAlive())
                { }
                else if (Globals.ScriptConfig.Alarms[(int)AlarmType.Stuck].Enabled)
                {
                    Waypoint waypoint = Globals.ScriptConfig.Waypoints[Globals.WaypointId];
                    if (Globals.ScriptConfig.CavebotStatus && 
                        (waypoint.Type == WaypointType.Stand || waypoint.Type==WaypointType.Node || waypoint.Type == WaypointType.Step || waypoint.Type == WaypointType.Not_Location_Goback || waypoint.Type == WaypointType.Not_Location_Goto_Label || waypoint.Type == WaypointType.If_Location_Goback || waypoint.Type == WaypointType.If_Location_Goto_Label)
                        && !Globals.ComboStatus && Objects.Client.TimeStopped >= 10000)
                        ticksStuck++;
                    else if (Globals.ScriptConfig.CavebotStatus &&
                        (waypoint.Type != WaypointType.Wait && waypoint.Type != WaypointType.Wait_PZ)
                        && !Globals.ComboStatus && Objects.Client.TimeStopped >= 120000)
                        ticksStuck++;
                    else
                        ticksStuck = 0;

                    if (ticksStuck >= 30)
                        alarmsRequested.Add(AlarmType.Stuck);

                }

                if (Globals.ScriptConfig.AlarmStatus)
                    alarmsRequested.AddRange(CheckAlarms());

                bool audioRequest = false;

                foreach (AlarmType alarm in alarmsRequested)
                {
                    if (Globals.ScriptConfig.Alarms[((int)alarm)].Enabled)
                    {
                        // DO LOG MESSAGE
                        Globals.Main.Log.addLog(alarm.ToString() + " " + extraInfo, true);

                        //
                        string actionString = Globals.ScriptConfig.Alarms[((int)alarm)].Action.Replace("()", "");

                        foreach (string action in actionString.Split(";"))
                        {
                            switch (action.Trim())
                            {
                                case "audio":
                                    audioRequest = true;
                                    break;
                                case "screenshot":
                                case "ss":
                                    if ((DateTime.Now - lastScreenshot).TotalMilliseconds > 10000)
                                    {
                                        Util.ScreenCapture.CaptureWindowToFile(Globals.Process.MainWindowHandle, string.Format("Screenshots/{0}_{1}.png", alarm.ToString(), DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")), System.Drawing.Imaging.ImageFormat.Png);
                                        lastScreenshot = DateTime.Now;
                                    }
                                    // TAKE SCREENSHOT
                                    break;
                                case "flash":
                                    WinApi.FlashWindow(Globals.Process.MainWindowHandle, true);
                                    break;
                                case "telegram":
                                case "tg":
                                    if (Globals.telegramBotToken != String.Empty && Globals.telegramUserId != String.Empty && (DateTime.Now - lastTelegramMessage).TotalMilliseconds > 5000)
                                    {
                                        Globals.TelegramBot.SendTextMessageAsync(Globals.telegramUserId, String.Format("{0}: {1}", Globals.Process.MainWindowTitle, alarm.ToString().Replace("_", " ")), Telegram.Bot.Types.Enums.ParseMode.Html);
                                        lastTelegramMessage = DateTime.Now;
                                    }
                                    break;
                                case "exit":
                                    Keyboard.PressKey(Keys.F19);
                                    Client.Say("!fps");
                                    Globals.Process?.Kill(true);
                                    Globals.Process = null;
                                    break;
                                case "exitall":
                                    foreach (Process p in Process.GetProcesses().Where(p=> p.MainWindowTitle.Contains("Tibia")))
                                    {
                                        p.Kill(true);
                                    }
                                    Globals.Process = null;
                                    break;
                                case "resetwpt":
                                    Globals.ComboStatus = false;
                                    Globals.WaypointId = 0;
                                    break;
                                case "target":
                                    if ((DateTime.Now - lastTarget).TotalMilliseconds > 5000)
                                    {
                                        Objects.Client.targetNear();
                                        lastTarget = DateTime.Now;
                                    }
                                    break;
                                case "safe":
                                    safeMode = true;
                                    break;
                                case "antibug":
                                    Objects.Player.Goto(Objects.Player.Position);
                                    break;
                                case "status":
                                    changedStatus = true;
                                    lastStatusMessage = Threads.ClientData.status;
                                    lastAlarmStatusMessage = alarm.ToString().Trim();
                                    Threads.ClientData.status = alarm.ToString().Trim();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                if (audioRequest)
                {
                    using (var soundPlayer = new SoundPlayer(@"Sounds\Siren.wav"))
                    {
                        soundPlayer.Play();
                    }
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (!changedStatus && Threads.ClientData.status == lastAlarmStatusMessage)
                    Threads.ClientData.status = lastStatusMessage;

                if (Globals.ScriptConfig.GeneralStatus)
                    Thread.Change(1000, Timeout.Infinite);
            }
        }


        private static List<AlarmType> CheckAlarms()
        {
            List<AlarmType> alarmsRequested = new List<AlarmType>();

            extraInfo = string.Empty;

            if (Globals.Process == null || Globals.Process.HasExited)
                return alarmsRequested;

            if (!Objects.Player.isLoggedIn)
            {
                if (Globals.ScriptConfig.Alarms[((int)AlarmType.Disconnected)].Enabled)
                {
                    alarmsRequested.Add(AlarmType.Disconnected);
                    Debug.WriteLine("Disconnected");
                }
                return alarmsRequested;
            }
            else if (!Objects.Player.isAlive())
            {
                if (Globals.ScriptConfig.Alarms[((int)AlarmType.Death)].Enabled)
                {
                    alarmsRequested.Add(AlarmType.Death);
                    Debug.WriteLine("Death");
                }   
                return alarmsRequested;
            }

            // FUNCTIONS THAT REQUIRES BATTLELIST
            if  (Globals.ScriptConfig.Alarms[(int)AlarmType.GM_On_Screen].Enabled || Globals.ScriptConfig.Alarms[(int)AlarmType.Player_On_Screen].Enabled || Globals.ScriptConfig.Alarms[(int)AlarmType.PK_On_Screen].Enabled || Globals.ScriptConfig.Alarms[(int)AlarmType.Has_Skull_Dangerous].Enabled)
            {
                List<Creature> creatures = Battlelist.getCreaturesOnScreen();
                if (Globals.ScriptConfig.Alarms[(int)AlarmType.GM_On_Screen].Enabled && creatures.Exists(c => c.Name.Contains("[") || gmNames.Contains(c.Name)))
                    alarmsRequested.Add(AlarmType.GM_On_Screen);

                if (Globals.ScriptConfig.Alarms[(int)AlarmType.Player_On_Screen].Enabled && creatures.Exists(c => c.Type == CreatureType.Player && c.Address != Player.Creature.Address))
                {
                    alarmsRequested.Add(AlarmType.Player_On_Screen);
                    Creature? c = creatures.FirstOrDefault(c => c.Type == CreatureType.Player && c.Address != Player.Creature.Address);
                    if (c != null)
                        extraInfo = c.Name;
                }

                if (Globals.ScriptConfig.Alarms[(int)AlarmType.PK_On_Screen].Enabled && creatures.Exists(c => c.Skull != PlayerSkulls.None && c.Address != Player.Creature.Address))
                    alarmsRequested.Add(AlarmType.PK_On_Screen);

                if (Globals.ScriptConfig.Alarms[(int)AlarmType.Has_Skull_Dangerous].Enabled && (Player.Creature.Skull == PlayerSkulls.White || Player.Creature.Skull == PlayerSkulls.Red || Player.Creature.Skull == PlayerSkulls.Black) && creatures.Exists(c => c.Type == CreatureType.Player && c.Address != Player.Creature.Address && c.HealthPc < 50))
                    alarmsRequested.Add(AlarmType.PK_On_Screen);
            }

            if (Globals.ScriptConfig.Alarms[(int)AlarmType.Local_Chat_Message].Enabled && Objects.ClientData.hasMessageLocalChat)
                alarmsRequested.Add(AlarmType.Local_Chat_Message);

            if (Globals.ScriptConfig.Alarms[(int)AlarmType.Has_Skull].Enabled && (Player.Creature.Skull == PlayerSkulls.White || Player.Creature.Skull == PlayerSkulls.Red || Player.Creature.Skull == PlayerSkulls.Black))
                alarmsRequested.Add(AlarmType.Has_Skull);

            // 
            if (Globals.ScriptConfig.Alarms[(int)AlarmType.Low_Cap].Enabled && Objects.Player.Cap <= 200)
                alarmsRequested.Add(AlarmType.Low_Cap);

            if (Globals.ScriptConfig.Alarms[(int)AlarmType.Low_Stamina].Enabled && Objects.Player.Stamina.TotalMinutes <= 14 * 60)
                alarmsRequested.Add(AlarmType.Low_Stamina);

            if (Globals.ScriptConfig.Alarms[(int)AlarmType.No_Imbue].Enabled && (DateTime.Now - lastCheckImbueTime).TotalMilliseconds >= 60 * 5 * 1000)
            {
                Objects.Client.lookAt(Equipment.Weapon);
                System.Threading.Thread.Sleep(500);
                List<string> messages = Objects.Client.getServerLogMessages();
                for (int i = 1; i <= 25; i++)
                {
                    if (i > messages.Count)
                        break;

                    string msg = messages[messages.Count - i].ToLower();

                    if (msg.Contains("imbuements:"))
                    {
                        lastCheckImbueTime = DateTime.Now;
                        hasImbue = msg.Contains("void");
                        break;
                    }
                }
            }
            else if (Globals.ScriptConfig.Alarms[(int)AlarmType.No_Imbue].Enabled && !hasImbue)
                alarmsRequested.Add(AlarmType.No_Imbue);


            return alarmsRequested;
        }


    }
}
