using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.ObjectTemples
{
    public class game
    {
        public List<rules> rules;
        public object value;
    }

    public class jvmArguments
    {
        public List<rules> rules;
        public string[] value;
    }

    public class rules
    {
        public string action;
        public os os;
        public features features;
    }
    public class features
    {
        public bool is_demo_user;
        public bool has_custom_resolution;
    }

    public class os
    {
        public string name;
    }
}
