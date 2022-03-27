using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDemo.Data
{
    public class DirInfo
    {
        public DirInfo(string path,string friendlyName)
        {
            FriendlyName = friendlyName;
            Path = path;
        }
        public string FriendlyName { get; set; }
        public string Path { get; set; }
    }
}
