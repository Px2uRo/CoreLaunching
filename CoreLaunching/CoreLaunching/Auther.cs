using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CoreLaunching.JsonTemplates;
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
        public void Auth(string Account,string Password)
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
        public void Auth(string Account,string Password,string AuthServer)
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
        public PlayerInfo MSAuth(string url)
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
                #region XBL(XboxLive) 验证
                Content = "{\"Properties\": {\"AuthMethod\": \"RPS\",\"SiteName\": \"user.auth.xboxlive.com\",\"RpsTicket\": \"" +MSARoot.Access_Token+ "\"},\"RelyingParty\": \"http://auth.xboxlive.com\",\"TokenType\": \"JWT\"}";
                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(@"https://user.auth.xboxlive.com/user/authenticate");
                request2.Method = "POST";
                byte[] byteArray2 = Encoding.UTF8.GetBytes(Content);
                request2.ContentLength = byteArray2.Length;
                request2.ContentType = "application/json";
                request2.Accept = "application/json";
                Stream dataStream2 = request2.GetRequestStream();
                dataStream2.Write(byteArray2, 0, byteArray2.Length);
                dataStream2.Close();
                WebResponse response2 = request2.GetResponse();
                dataStream2 = response2.GetResponseStream();
                StreamReader reader2 = new StreamReader(dataStream2);
                responseFromServer = reader2.ReadToEnd();
                var XBLRoot = JsonConvert.DeserializeObject<XBLResponse>(responseFromServer);
                #endregion
                #region XSTS 验证
                Content = "{ \"Properties\": {\"SandboxId\": \"RETAIL\",\"UserTokens\":[\"" +XBLRoot.Token+ "\"]},\"RelyingParty\": \"rp://api.minecraftservices.com/\",\"TokenType\": \"JWT\"}";
                HttpWebRequest request3 = (HttpWebRequest)WebRequest.Create(@"https://xsts.auth.xboxlive.com/xsts/authorize");
                request3.Method = "POST";
                byte[] byteArray3 = Encoding.UTF8.GetBytes(Content);
                request3.ContentLength = byteArray3.Length;
                request3.ContentType = "application/json";
                request3.Accept = "application/json";
                Stream dataStream3 = request3.GetRequestStream();
                dataStream3.Write(byteArray3, 0, byteArray3.Length);
                dataStream3.Close();
                WebResponse response3 = request3.GetResponse();
                dataStream3 = response3.GetResponseStream();
                StreamReader reader3 = new StreamReader(dataStream3);
                responseFromServer = reader3.ReadToEnd();
                var XSTSRoot = JsonConvert.DeserializeObject<XBLResponse>(responseFromServer);
                #endregion
                #region 登录 Minecraft
                Content = "{\"identityToken\":\"XBL3.0 x=" + XSTSRoot.DisplayClaims.Xui[0].Uhs + ";" + XSTSRoot.Token + "\"}";
                HttpWebRequest request4 = (HttpWebRequest)WebRequest.Create(@"https://api.minecraftservices.com/authentication/login_with_xbox");
                request4.Method = "POST";
                byte[] byteArray4 = Encoding.UTF8.GetBytes(Content);
                request4.ContentLength = byteArray4.Length;
                request4.ContentType = "application/json";
                request4.Accept = "application/json";
                Stream dataStream4 = request4.GetRequestStream();
                dataStream4.Write(byteArray4, 0, byteArray4.Length);
                dataStream4.Close();
                WebResponse response4 = request4.GetResponse();
                dataStream4 = response4.GetResponseStream();
                StreamReader reader4 = new StreamReader(dataStream4);
                responseFromServer = reader4.ReadToEnd();
                var login_with_xboxResponseRoot = JsonConvert.DeserializeObject<Login_With_XboxResponse>(responseFromServer);
                #endregion
                #region 检查游戏所有权
                HttpWebRequest request5 = (HttpWebRequest)WebRequest.Create(@"https://api.minecraftservices.com/entitlements/mcstore");
                request5.Method = "GET";
                request5.Headers.Add("Authorization: Bearer "+login_with_xboxResponseRoot.Access_Token);
                WebResponse response5 = request5.GetResponse();
                var dataStream5 = response5.GetResponseStream();
                StreamReader reader5 = new StreamReader(dataStream5);
                responseFromServer = reader5.ReadToEnd();
                var OwnershipRoot = JsonConvert.DeserializeObject<Ownership>(responseFromServer);
                #endregion
                #region 获取个人资料
                if (OwnershipRoot.Items.Count != 0)
                {
                    HttpWebRequest request6 = (HttpWebRequest)WebRequest.Create(@"https://api.minecraftservices.com/minecraft/profile");
                    request6.Method = "GET";
                    request6.Headers.Add("Authorization: Bearer " + login_with_xboxResponseRoot.Access_Token);
                    WebResponse response6 = request6.GetResponse();
                    var dataStream6 = response6.GetResponseStream();
                    StreamReader reader6 = new StreamReader(dataStream6);
                    responseFromServer = reader6.ReadToEnd();
                    PlayerInfo playerInfoRoot = JsonConvert.DeserializeObject<PlayerInfo>(responseFromServer);
                    playerInfoRoot.Access_Token = login_with_xboxResponseRoot.Access_Token;
                    return playerInfoRoot;
                }
                else
                {
                    return null;
                    throw new Exception("这号没得 Minecraft");
                }
                #endregion
            }
            catch (Exception ex)
            {
                return null;
                throw new Exception(ex.Message);
            }
        }

    }
}