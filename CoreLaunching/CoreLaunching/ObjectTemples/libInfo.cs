
namespace CoreLaunching.ObjectTemples
{
    public class artifact
    {
        public string path;
        public string sha1;
        public string size;
        public string url;
    }
    public class downloads
    {
        public artifact artifact;
        public classifiers classifiers;
    }

    public class classifiers
    {
        public artifact natives_linux;
        public artifact natives_osx;
        public artifact natives_windows;
        public artifact natives_windows_x64;
        public artifact natives_windows_x86;
    }
    public class natives
    {
       public string windows;
       public string osx;
       public string linux;
    }

    public class libInfo
    {
        public downloads downloads;
        public natives natives;
        public string name;
    }
}
