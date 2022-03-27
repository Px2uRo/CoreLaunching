using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDemo.Data
{
    public class LegacyPlayerInfo
    {
        public LegacyPlayerInfo(string name,string uuid)
        {
            Name = name;
            Uuid = uuid;
        }
        public string Name { get; set; }
        public string Uuid { get; set; }
    }
}
