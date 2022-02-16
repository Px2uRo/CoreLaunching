using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching
{
    public class DirectorySystem
    {
        public void DirSys(string Target)
        {
            DirectoryInfo TheFolder = new DirectoryInfo(Target);
            try
            {
                //判断所指的文件夹/文件是否存在  
                if (!TheFolder.Exists)
                    return;
                DirectoryInfo dirD = TheFolder as DirectoryInfo;
                FileSystemInfo[] files = dirD.GetFileSystemInfos();//获取文件夹下所有文件和文件夹  
                //对单个FileSystemInfo进行判断，如果是文件夹则进行递归操作  
                foreach (FileSystemInfo FSys in files)
                {
                    FileInfo fileInfo = FSys as FileInfo;

                    if (fileInfo != null)
                    {
                        //是文件时执行命令
                        foreach (FileInfo jar in TheFolder.GetFiles("*.jar"))
                        {
                            string MCJarName = jar.Name.ToString();

                            foreach (FileInfo json in TheFolder.GetFiles(MCJarName))
                            {
                                Console.WriteLine(json);
                            }
                        }
                    }
                    else
                    {
                        //如果是文件夹，则进行递归调用 
                        string pp = FSys.Name;
                        DirSys(Target + "\\" + FSys.ToString()); 
                        Console.WriteLine(pp);
                    }
                }




            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
    }
}
