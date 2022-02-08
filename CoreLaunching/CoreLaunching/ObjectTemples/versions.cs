using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.ObjectTemples
{
    /// <summary>
    /// 最新版
    /// </summary>
    public class latest
    {
        /// <summary>
        /// 最新发布版
        /// </summary>
        public string release;
        /// <summary>
        /// 最新快照
        /// </summary>
        public string snapshot;
    }
    /// <summary>
    /// 版本详细信息
    /// </summary>
    public class versions
    {
        /// <summary>
        /// 版本id
        /// </summary>
        public string id;
        /// <summary>
        /// 版本类型
        /// </summary>
        public string type;
        /// <summary>
        /// 此版本 JSON 的地址
        /// </summary>
        public string url;
        /// <summary>
        /// 该版本表单源添加此版本的时间
        /// </summary>
        public string time;
        /// <summary>
        /// 该版本发布时间
        /// </summary>
        public string releaseTime;
    }
    /// <summary>
    /// 版本需要的库
    /// </summary>
    public class libraries
    {
        /// <summary>
        /// 下载键
        /// </summary>
        public downloads downloads;
        /// <summary>
        /// 动态库键
        /// </summary>
        public natives natives;
        /// <summary>
        /// TODO:求写注释
        /// </summary>
        public extract extract;
        public string name;
    }
    /// <summary>
    /// Minecraft 1.13 之后加入此键，旧版为MinecraftArguments
    /// </summary>
    public class arguments
    {
        public List<game> game;
        public List<jvmArguments> jvm;
    }
}
