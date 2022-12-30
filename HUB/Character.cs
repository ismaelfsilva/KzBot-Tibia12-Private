using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUB
{
    public class Character
    {
        public int id { get; set; }
        public string name { get; set; }
        public string vocation { get; set; }
        public int level { get; set; }
        public int owner { get; set; }
        public string server { get; set; }
        public string account { get; set; }
        public string password { get; set; }
        public int balance { get; set; }
        public int last_stamina { get; set; }
        public string last_online { get; set; }
        public string time_online { get; set; }
        public int status { get; set; }
        public string script { get; set; }
        public string warning { get; set; }

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
                int recoveredStamina = (int)Math.Floor((DateTime.Now - lastOnlineTime).TotalMinutes / 6);
                TimeSpan actualStamina = TimeSpan.FromMinutes(last_stamina) + TimeSpan.FromMinutes(recoveredStamina);
                int totalHours = (int)Math.Floor(actualStamina.TotalHours);
                int minutes = actualStamina.Minutes;

                if (totalHours >= 42)
                    lvItem.SubItems.Add("42:00");
                else
                    lvItem.SubItems.Add($"{totalHours:D2}:{minutes:D2}");

                lvItem.SubItems.Add(lastOnlineTime.ToString());

                return lvItem;
            }
        }
    }
}
