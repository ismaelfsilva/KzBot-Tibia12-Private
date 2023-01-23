using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUB.Classes
{
    public class Character
    {
        public int id { get; set; } = -1;
        public string name { get; set; } = string.Empty;
        public string vocation { get; set; } = "None";
        public int level { get; set; } = -1;
        public int owner { get; set; } = -1;
        public string server { get; set; } = string.Empty;
        public string account { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public int char_index { get; set; } = 0;
        public int balance { get; set; } = 0;
        public int last_stamina { get; set; } = 0;
        public string last_online { get; set; } = string.Empty;
        public int time_online { get; set; } = 0;
        public int status { get; set; } = 0;
        public string script { get; set; } = string.Empty;
        public string warning { get; set; } = string.Empty;
        public int imbuement_time { get; set; } = 0;
        public string status_update_time { get; set; } = DateTime.MinValue.ToString();
        public string script_status { get; set; } = string.Empty;
        public List<string> system_warning { get; set; } = new List<string>();

        public TimeSpan actual_stamina
        {
            get
            {
                DateTime lastOnlineTime = DateTime.Parse(last_online);
                int recoveredStamina = (int)Math.Floor((DateTime.Now - lastOnlineTime).TotalMinutes / 6);
                TimeSpan actualStamina = TimeSpan.FromMinutes(last_stamina) + TimeSpan.FromMinutes(recoveredStamina);

                return actualStamina;
            }
        }

        public ListViewItem ListViewItem
        {
            get
            {
                ListViewItem lvItem = new ListViewItem();
                lvItem.Text = id.ToString();
                lvItem.SubItems.Add(name);
                lvItem.SubItems.Add(level.ToString());
                lvItem.SubItems.Add(vocation);
                lvItem.SubItems.Add(script.ToString());
                lvItem.SubItems.Add(balance.ToString());

                DateTime lastOnlineTime = DateTime.Parse(last_online);
                int recoveredStamina = (int)Math.Floor((DateTime.Now - lastOnlineTime).TotalMinutes / Program.Config.Servers.FirstOrDefault(s=> s.name.ToLower().Trim() == server.ToLower().Trim()).staminaRecoveryDelay);
                TimeSpan actualStamina = TimeSpan.FromMinutes(last_stamina) + TimeSpan.FromMinutes(recoveredStamina);
                int totalHours = (int)Math.Floor(actualStamina.TotalHours);
                int minutes = actualStamina.Minutes;

                if (totalHours >= 42)
                    lvItem.SubItems.Add("42:00");
                else
                    lvItem.SubItems.Add($"{totalHours:D2}:{minutes:D2}");


                if (imbuement_time >= 0)
                {
                    TimeSpan imbueTimeSpan = TimeSpan.FromMinutes(imbuement_time);
                    lvItem.SubItems.Add(string.Format("{0}:{1}", imbueTimeSpan.TotalHours >= 1 ? imbueTimeSpan.Hours.ToString("D2") : "00", imbueTimeSpan.Minutes >= 1 ? imbueTimeSpan.Minutes.ToString("D2") : "00"));
                }
                else
                    lvItem.SubItems.Add("Unknown");

                lvItem.SubItems.Add(lastOnlineTime.ToString());
                DateTime lastStatusUpdateTime = DateTime.Parse(status_update_time);
                lvItem.SubItems.Add(string.Format("[{0}] {1}", lastStatusUpdateTime, script_status.ToString()));

                return lvItem;
            }
        }
    }
}
