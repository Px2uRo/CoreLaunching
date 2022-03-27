using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace CLDemo.Data
{
    public class JavaPathInfo
    {
        public string JavaPath { get; set; }
        public string MajorVersion { get; set; }
        public JavaPathInfo(string JavaPath)
        {
            this.JavaPath = JavaPath;
            this.MajorVersion = FileVersionInfo.GetVersionInfo(JavaPath).FileMajorPart.ToString();
        }
    }
}
