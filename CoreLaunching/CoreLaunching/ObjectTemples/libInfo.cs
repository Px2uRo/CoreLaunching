
namespace CoreLaunching.ObjectTemples
{
    #region 本地版本 JSON 管理class
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
        public artifact natives_windows_64;
        public artifact natives_windows_86;
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
    #endregion
}
