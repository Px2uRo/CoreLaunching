using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLaunching.Down.Helpers
{
    public static class FanFileHelper
    {
        #region 打开文件夹
        /// <summary>通过一定可以构建出指定文件夹</summary>
        public static void CreateFolder(string path)
        {
            if (!System.IO.Directory.Exists(path))
                CreateFolderCore(path);
        }

        /// <summary>创建文件夹的核心递归代码。</summary>
        private static void CreateFolderCore(string path)
        {
            var parentPath = System.IO.Path.GetDirectoryName(path);
            if (parentPath == null)
            {
                throw new ArgumentException($"无法找到{path}该磁盘目录");
            }
            if (!System.IO.Directory.Exists(parentPath))
            {
                CreateFolderCore(parentPath);
            }
            System.IO.Directory.CreateDirectory(path);
        }

        #endregion

        #region 打开资源管理器

        /// <summary>打开文件资源管理器并且找到指定文件。</summary>
        public static void StartProcessAndSelectFile(string filePath)
        {
            var proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "explorer";
            proc.StartInfo.Arguments = @"/select," + filePath;
            proc.Start();
            //System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(viewModel.ExportFilePath));
        }
        #endregion
    }

}
