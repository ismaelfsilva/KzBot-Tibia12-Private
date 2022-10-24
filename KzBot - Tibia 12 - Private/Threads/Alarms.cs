using KzBot.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace KzBot.Threads
{
    public static class Alarms
    {
        public static System.Threading.Timer Thread = new System.Threading.Timer(AlarmThread, null, Timeout.Infinite, Timeout.Infinite);
        private static DateTime lastScreenshot = DateTime.MinValue;

        private static List<string> gmNames = new List<string>() { "Lucas", "worthdavi", "Shivas"};
        private static int ticksStuck = 0;
        private static Position lastCheckPosition = new Position() { X = 0, Y = 0, Z = 0 };

        private static DateTime lastTelegramMessage = DateTime.MinValue;

        private static async void AlarmThread(object? state)
        {
            Thread.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                if (!Globals.Config.GeneralStatus)
                    return;

                List<AlarmType> alarmsRequested = new List<AlarmType>();

                if (Globals.Process == null || Globals.Process.HasExited || Globals.Process.MainWindowTitle == "Tibia")
                { }
                else if (Globals.Config.Alarms[(int)AlarmType.Stuck].Enabled)
                {
                    Position playerPos = Objects.Player.Position;
                    Waypoint waypoint = Globals.Config.Waypoints[Globals.WaypointId];
                    if (Globals.Config.CavebotStatus && playerPos == lastCheckPosition && (waypoint.Type != WaypointType.Sell_All && waypoint.Type != WaypointType.Wait_PZ && waypoint.Type != WaypointType.Wait && waypoint.Type != WaypointType.Buy_Refill) && !Globals.ComboStatus && ++ticksStuck >= 30)
                        alarmsRequested.Add(AlarmType.Stuck);
                    else if (playerPos != lastCheckPosition)
                    {
                        lastCheckPosition = playerPos;
                        ticksStuck = 0;
                    }
                }

                if (Globals.Config.AlarmStatus)
                    alarmsRequested.AddRange(CheckAlarms());

                bool audioRequest = false;

                foreach (AlarmType alarm in alarmsRequested)
                {
                    if (Globals.Config.Alarms[((int)alarm)].Enabled)
                    {
                        // DO LOG MESSAGE

                        //
                        string actionString = Globals.Config.Alarms[((int)alarm)].Action.Replace("()", "");

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
                                    Client.Say("!fps");
                                    Globals.Process?.Kill(true);
                                    Globals.Process = null;
                                    break;
                                case "exitall":
                                    foreach (Process p in Process.GetProcessesByName("client"))
                                    {
                                        p.Kill(true);
                                    }
                                    Globals.Process = null;
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
                if (Globals.Config.GeneralStatus)
                    Thread.Change(1000, Timeout.Infinite);
            }
        }

        private static List<AlarmType> CheckAlarms()
        {
            List<AlarmType> alarmsRequested = new List<AlarmType>();

            if (Globals.Process == null || Globals.Process.HasExited || Globals.Process.MainWindowTitle == "Tibia")
            {
                if (Globals.Config.Alarms[((int)AlarmType.Disconnected)].Enabled)
                {
                    alarmsRequested.Add(AlarmType.Disconnected);
                    Debug.WriteLine("Disconnected");
                }
                return alarmsRequested;
            }
            else if (!Objects.Player.isAlive())
            {
                if (Globals.Config.Alarms[((int)AlarmType.Death)].Enabled)
                {
                    alarmsRequested.Add(AlarmType.Death);
                    Debug.WriteLine("Death");
                }   
                return alarmsRequested;
            }

            if (!Objects.Player.isLoggedIn)
                return alarmsRequested;

            // FUNCTIONS THAT REQUIRES BATTLELIST
            if  (Globals.Config.Alarms[(int)AlarmType.GM_On_Screen].Enabled || Globals.Config.Alarms[(int)AlarmType.Player_On_Screen].Enabled || Globals.Config.Alarms[(int)AlarmType.PK_On_Screen].Enabled || Globals.Config.Alarms[(int)AlarmType.Has_Skull_Dangerous].Enabled)
            {
                List<Creature> creatures = Battlelist.getCreaturesOnScreen();
                if (Globals.Config.Alarms[(int)AlarmType.GM_On_Screen].Enabled && creatures.Exists(c => c.Name.Contains("[") || gmNames.Contains(c.Name)))
                    alarmsRequested.Add(AlarmType.GM_On_Screen);

                if (Globals.Config.Alarms[(int)AlarmType.Player_On_Screen].Enabled && creatures.Exists(c => c.Type == CreatureType.Player && c.Address != Player.Creature.Address && !Globals.Accounts.List.Exists(a=> a.Character == c.Name)))
                    alarmsRequested.Add(AlarmType.Player_On_Screen);

                if (Globals.Config.Alarms[(int)AlarmType.PK_On_Screen].Enabled && creatures.Exists(c => c.Skull != PlayerSkulls.None && c.Address != Player.Creature.Address))
                    alarmsRequested.Add(AlarmType.PK_On_Screen);

                if (Globals.Config.Alarms[(int)AlarmType.Has_Skull_Dangerous].Enabled && (Player.Creature.Skull == PlayerSkulls.White || Player.Creature.Skull == PlayerSkulls.Red || Player.Creature.Skull == PlayerSkulls.Black) && creatures.Exists(c => c.Type == CreatureType.Player && c.Address != Player.Creature.Address && c.HealthPc < 50))
                    alarmsRequested.Add(AlarmType.PK_On_Screen);
            }

            if (Globals.Config.Alarms[(int)AlarmType.Local_Chat_Message].Enabled && Objects.ClientData.hasMessageLocalChat)
                alarmsRequested.Add(AlarmType.Local_Chat_Message);

            if (Globals.Config.Alarms[(int)AlarmType.Has_Skull].Enabled && (Player.Creature.Skull == PlayerSkulls.White || Player.Creature.Skull == PlayerSkulls.Red || Player.Creature.Skull == PlayerSkulls.Black))
                alarmsRequested.Add(AlarmType.Has_Skull);

            // 
            if (Globals.Config.Alarms[(int)AlarmType.Low_Cap].Enabled && Objects.Player.Cap <= 200)
                alarmsRequested.Add(AlarmType.Low_Cap);

            if (Globals.Config.Alarms[(int)AlarmType.Low_Stamina].Enabled && Objects.Player.Stamina.TotalMinutes <= 14 * 60)
                alarmsRequested.Add(AlarmType.Low_Stamina);
                       
            return alarmsRequested;
        }
    }
}
