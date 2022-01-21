using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;

namespace CoreLaunching
{
        /// <summary>
        /// Manage Thread Files
        /// </summary>
    public class SuperDownloader
    {
        int thread;
        string url;
        FileStream fs;
        HttpWebRequest request;
        Stream ns;
        byte[] nbytes;
        int nreadsize;
        /// <summary>
        /// Muitl Thread Download void
        /// </summary>
        /// <param name="url">TargetFile</param>
        /// <param name="threads">the number of the threads</param>
        /// <param name="targetFolder">targetFolder</param>
        public void Download(string url,int threads,string localTargetFolder)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            localTargetFolder = localTargetFolder + @"\" + Path.GetFullPath(url);
            int currectThread = 0;
            string[] currectThreadtmpFileName;
            bool[] threadIsDone;
            bool MIXED;
            byte[] nbytes = new byte[512];
            Stream ns = null;
            int downloadedSize;
            int[] byteStartEveryThread;
            int[] byteTotalEveryThread;
            Console.WriteLine("Thread {0} is bigenning to recive", currectThread.ToString());
            FileStream fs = new FileStream(localTargetFolder,FileMode.Create);
            try
            {
                int sizeInAThreads = 
                    (int)request.GetResponse().ContentLength / threads;//平均分配
                int sizeInAthreadsEnd =sizeInAThreads + (int)request.GetResponse().ContentLength % threads;//Yushu 部分由最后一个线程完成
                #region Genju xiancheng shu chushihua de shuju
                currectThreadtmpFileName =new string[threads];
                threadIsDone=new bool[threads];
                byteTotalEveryThread=new int[threads];
                byteStartEveryThread=new int[threads];
                #endregion
                for (int i = 0; i < threads; i++)
                {
                    threadIsDone[i] = false;
                    currectThreadtmpFileName[i] = Path.GetFileName(localTargetFolder)+threads.ToString()+".tmp";
                    if (i < threads - 1)
                    {
                        byteStartEveryThread[i] = sizeInAThreads * i;
                        byteTotalEveryThread[i] = sizeInAThreads - 1;
                    }
                    else
                    {
                        byteStartEveryThread[i] = sizeInAThreads * i;
                        byteTotalEveryThread[i]=sizeInAthreadsEnd - 1;
                    }
                }
                request.AddRange(byteStartEveryThread[currectThread], byteStartEveryThread[currectThread] + byteTotalEveryThread[currectThread]);
                Thread[] threadk = new Thread[threads];
                SuperDownloader[] superDownloader = new SuperDownloader[threads];
                for (int i = 0; i < threads; i++)
                {
                    superDownloader[i] = new SuperDownloader(this,j);
                    threadk[i] = Thread(new ThreadStart(superDownloader[i].Download));
                    threadk[i].Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
