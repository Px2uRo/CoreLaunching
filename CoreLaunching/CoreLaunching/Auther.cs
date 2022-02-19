using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CoreLaunching.ObjectTemplates;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;

namespace CoreLaunching.Auth
{
    public class Auther
    {
        public Auther()
        {

        }
        public void auth(string Account,string Password)
        {
            try
            {
                string Data = "{\"agent\": {\"name\": \"Minecraft\",\"version\": 1},\"username\":\"" + Account + "\",\"password\":\"" + Password + "\",\"requestUser\":true}";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://authserver.mojang.com/authenticate");
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(Data);
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/json";
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("403"))
                {
                    Console.WriteLine("账号密码不对");
                }
            }
        }
        public void auth(string Account,string Password,string AuthServer)
        {
            try
            {
                string Data = "{\"agent\": {\"name\": \"Minecraft\",\"version\": 1},\"username\":\"" + Account + "\",\"password\":\"" + Password + "\",\"requestUser\":true}";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AuthServer);
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(Data);
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/json";
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("403"))
                {
                    Console.WriteLine("账号密码不对");
                }
            }
        }
    }
    public class MSAuther
    {
        public Uri LogInUrl = new Uri(@"https://login.live.com/oauth20_authorize.srf?client_id=00000000402b5328&response_type=code&scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf");
        public string MSAuth(string url)
        {
            try
            {
                #region 微软登录
                string Content = "client_id=00000000402b5328" +
                "&code=" + url.Replace("https://login.live.com/oauth20_desktop.srf?code=", "") +
                "&grant_type=authorization_code" +
                "&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf" +
                "&scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://login.live.com/oauth20_token.srf");
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(Content);
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                var MSARoot = JsonConvert.DeserializeObject<MSAuthResponse>(responseFromServer);
                #endregion
                return MSARoot.access_token;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}