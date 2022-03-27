using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDemo.Data
{
    /// <summary>
    /// 公共的数据访问区，是静态对象，整个程序运行全程中，唯一存在。
    /// </summary>
    public static class GeneralData
    {
        public static ObservableCollection<LegacyPlayerInfo> LegacyPlayerInfos { get; set; }
        public static ObservableCollection<DirInfo> DirInfos { get; set; }
        public static int SelectedLegacyListIndex { get; set; }
        public static ObservableCollection<JavaPathInfo> JavaPathInfos { get; set; }
        public static ObservableCollection<GameInfo> GameInfos { get; set; }
        public static int SelectedJavaPathListIndex { get; set; }
        public static int SelectedDirInfoListIndex { get; set; }
        public static int SelectedGameListIndex { get; set; }
        static GeneralData()
        {
            LegacyPlayerInfos=new ObservableCollection<LegacyPlayerInfo>();
            DirInfos=new ObservableCollection<DirInfo>();
            JavaPathInfos=new ObservableCollection<JavaPathInfo>();
            GameInfos = new ObservableCollection<GameInfo>();
            SelectedGameListIndex = 0;
        }
    }
}
