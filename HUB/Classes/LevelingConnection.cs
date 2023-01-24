using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUB.Classes
{
    public class LevelingConnection
    {
        public bool enabled { get; set; } = false;
        public bool createCharacter { get; set; } = true;
        public int levelMax { get; set; } = 150;


        public string vocation { get; set; } = "RP";

        public string server { get; set; } = string.Empty;
        public string script { get; set; } = string.Empty;

        public ListViewItem ListViewItem
        {
            get
            {
                ListViewItem lvItem = new ListViewItem();
                lvItem.Checked = enabled;
                lvItem.SubItems.Add(createCharacter.ToString());
                lvItem.SubItems.Add(server);
                lvItem.SubItems.Add(script);

                return lvItem;
            }
        }
    }
}
