using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HUB.Classes
{
    public class ScriptConnection
    {
        public bool enabled { get; set; } = false;
        public bool requiresImbuement { get; set; } = true;
        public int maxBalance { get; set; } = -1;
        public int minutesToWaitOnBan { get; set; } = 120;
        public int minMinutesBetweenScripts { get; set; } = 5;
        public int minLevel { get; set; } = 5;
        public string server { get; set; } = string.Empty;
        public string script { get; set; } = string.Empty;

        [XmlIgnore]
        public DateTime lastConnection { get; set; } = DateTime.MinValue;


        public ListViewItem ListViewItem
        {
            get
            {
                ListViewItem lvItem = new ListViewItem();
                lvItem.Checked = enabled;
                lvItem.SubItems.Add(requiresImbuement.ToString());
                lvItem.SubItems.Add(server);
                lvItem.SubItems.Add(script);

                return lvItem;
            }
        }
    }
}
