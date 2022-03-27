using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.IO;
using CoreLaunching;
using GD = CLDemo.Data.GeneralData;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;

namespace CLDemo.Modelview
{
    public class LaunchGameViewModel : VisibleViewModel
    {

        #region Commands

        public class LaunchGameCommand : ICommand
        {
            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                if (GD.DirInfos[GD.SelectedDirInfoListIndex] != null)
                {
                    if (GD.GameInfos[GD.SelectedGameListIndex].JsonPath != null)
                    {
                        if (File.Exists(GD.GameInfos[GD.SelectedGameListIndex].JsonPath))
                        {
                            if (File.Exists(GD.GameInfos[GD.SelectedGameListIndex].JsonPath.Replace(".json", ".jar"))==false)
                            {
                                    StreamReader loader = new StreamReader(GD.GameInfos[GD.SelectedGameListIndex].JsonPath);
                                    JsonTextReader reader = new JsonTextReader(loader);
                                    var jOb = (JObject)JToken.ReadFrom(reader);
                                    JObject tmp = (JObject)jOb["downloads"];
                                    JObject tmp2 = (JObject)tmp["client"];
                                    string url = tmp2["url"].ToString();
                                    loader.Close();
                                    reader.Close();
                                    Stream responseStream = (
                                        (WebRequest.Create(url) as HttpWebRequest).GetResponse() 
                                        as HttpWebResponse).GetResponseStream();
                                    FileStream fileStream = new FileStream(GD.GameInfos[GD.SelectedGameListIndex].JsonPath.Replace(".json", ".jar"), FileMode.Create);
                                    byte[] array = new byte[1024];
                                    for (int num = responseStream.Read(array, 0, array.Length); num > 0; num = responseStream.Read(array, 0, array.Length))
                                    {
                                        fileStream.Write(array, 0, num);
                                    }
                                    fileStream.Close();
                                    responseStream.Close();
                                    StaticVoids.RefreshGameInfoList(GD.DirInfos[GD.SelectedDirInfoListIndex].Path);
                            }
                            if (GD.LegacyPlayerInfos.Count == 0)
                            {
                                MessageBox.Show("至少添加一个用户");
                            }
                            else
                            {
                                if (GD.JavaPathInfos.Count == 0)
                                {
                                    MessageBox.Show("至少添加一个 Java");
                                }
                                else
                                {
                                    #region Objects
                                    string natives = Path.Combine
                                        (GD.DirInfos[GD.SelectedDirInfoListIndex].Path, "natives");
                                    string assets = Path.Combine
                                        (GD.DirInfos[GD.SelectedDirInfoListIndex].Path, "assets");
                                    string libraries = Path.Combine
                                        (GD.DirInfos[GD.SelectedDirInfoListIndex].Path, "libraries");
                                    StreamReader loader = new StreamReader(GD.GameInfos[GD.SelectedGameListIndex].JsonPath);
                                    JsonTextReader reader = new JsonTextReader(loader);
                                    var jOb = (JObject)JToken.ReadFrom(reader);
                                    #endregion
                                    JObject tmp = (JObject)jOb["assetIndex"];
                                    string id = tmp["id"].ToString() + ".json";
                                    string url = tmp["url"].ToString();
                                    loader.Close();
                                    reader.Close();

                                    string assets_index = Path.Combine
                                        (GD.DirInfos[GD.SelectedDirInfoListIndex].Path, "assets\\indexs",id);
                                    if (Directory.Exists(natives) == false)
                                    {
                                        Directory.CreateDirectory(natives);
                                    }
                                    if (Directory.Exists(libraries) == false)
                                    {
                                        Directory.CreateDirectory(libraries);
                                    }
                                    if (File.Exists(GD.GameInfos[GD.SelectedGameListIndex].JsonPath) ==false)
                                    {
                                        Stream responseStream = (
                                        (WebRequest.Create(url) as HttpWebRequest).GetResponse()
                                        as HttpWebResponse).GetResponseStream();
                                        FileStream fileStream = new FileStream(assets_index, FileMode.CreateNew);
                                        byte[] array = new byte[1024];
                                        for (int num = responseStream.Read(array, 0, array.Length); num > 0; num = responseStream.Read(array, 0, array.Length))
                                        {
                                            fileStream.Write(array, 0, num);
                                        }
                                        fileStream.Close();
                                        responseStream.Close();
                                    }
                                    CoreLaunching.GameArgsInfo gameArgsInfo = new GameArgsInfo(GD.LegacyPlayerInfos[GD.SelectedLegacyListIndex].Name,
                                        GD.DirInfos[GD.SelectedDirInfoListIndex].Path,
                                        assets, GD.GameInfos[GD.SelectedGameListIndex].JsonPath,
                                        GD.LegacyPlayerInfos[GD.SelectedLegacyListIndex].Uuid,
                                        "0000000000000003A98F501BCC514FFA", "", "",
                                        "Legacy", jOb["type"].ToString(),
                                        600, 600
                                        );
                                    CoreLaunching.JVMArgsInfo jVMArgsInfo = new JVMArgsInfo(natives,
                                        "CLDemo", new Version(1, 0));
                                    CoreLaunching.Launcher launcher = new Launcher(GD.GameInfos[GD.SelectedGameListIndex].JsonPath,
                                        GD.JavaPathInfos[GD.SelectedJavaPathListIndex].JavaPath,
                                        jVMArgsInfo,libraries,
                                        GD.GameInfos[GD.SelectedGameListIndex].JarPath,
                                        natives,gameArgsInfo);
                                    try
                                    {
                                        launcher.Launch(false, Launcher.MyPlatforms.Windows, "disallow", "x64", 24, 2048, "-XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True -Dlog4j2.formatMsgNoLookups=true");
                                    }
                                    catch(Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("您可能删除了此处的 JSON 而我还没来得及做刷新按钮，要怎么办呢？我布吉岛哇。");
                        }
                    }
                    else
                    {
                        MessageBox.Show("无法解析到这里面有 JSON");
                    }
                }
                else
                {
                    MessageBox.Show("请创建或选择一个游戏目录");
                }
            }
        }

        #endregion

        public LaunchGameCommand LaunchGameCmd { get; private set; }

        public LaunchGameViewModel(LaunchPageViewModel launchViewModel):base(launchViewModel)
        {
            LaunchGameCmd =new LaunchGameCommand();
        }

    }
}
