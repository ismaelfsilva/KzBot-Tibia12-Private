using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUB.Classes
{
    public class LogMessage
    {

        public int id { get; set; }
        public int owner { get; set; }
        public int char_id { get; set; }
        public string message { get; set; } = string.Empty;
        public string time { get; set; } = string.Empty;
        public int gravity { get; set; } = 0;

        public ListViewItem ListViewItem
        {
            get
            {
                Character character = Program.Characters.FirstOrDefault(c => c.id == char_id);

                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = id.ToString();
                listViewItem.SubItems.Add(character.name);
                listViewItem.SubItems.Add(character.server);
                listViewItem.SubItems.Add(time.ToString());
                listViewItem.SubItems.Add(message);

                if (gravity == 1)
                    listViewItem.BackColor = Color.Green;
                else if (gravity == 2)
                    listViewItem.BackColor = Color.Yellow;
                else if (gravity == 3)
                    listViewItem.BackColor = Color.Red;

                return listViewItem;
            }
        }
    }
}
