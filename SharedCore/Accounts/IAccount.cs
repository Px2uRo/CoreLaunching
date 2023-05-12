using CoreLaunching.JsonTemplates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CoreLaunching.Accounts;
public interface IAccount
{
    public string UserName { get; set; }
    public string Uuid { get; set; }
    public string AccessToken { get; set; }
    public string UserProperties { set; get; }
    public string UserType { get; set; }
    public string ClientID { get; set; }
    public string Xuid { get; set; }
    public string JVMArgs { get; set; }
    public bool SingUp(out string errorInfo);
}

public class UnitedPassportAccount : IAccount
{
    public override string ToString()
    {
        return $"{EmailAddress}: {UserName} ({ServerID})";
    }
    public string ServerID { get; set; }
    [JsonIgnore]
    public string Password { get; set; }
    public string EmailAddress { get; set; }
    public string ClientToken { get; set; }
    public string UserName { get; set; }
    public string Uuid { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string AccessToken { get; set; }
    public string UserProperties { get; set; }
    public string UserType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ClientID { get; set; }
    public string Xuid { get; set; }
    [JsonIgnore]
    public string Nide8AuthPath { get; set; }
    public string JVMArgs { get => $"-Dnide8auth.client=true -javaagent:\"{Nide8AuthPath}\"={ServerID}"; set => throw new NotImplementedException(); }
    public UnitedPassportAccount()
    {
        ServerID = string.Empty;
        UserName = "${auth_player_name}";
        Uuid = "${auth_uuid}";
        AccessToken = "${auth_access_token}";
        UserType = "${user_type}";
        UserProperties = "{}";
        Xuid = "${auth_xuid}";
        ClientID = "${clientID}";
        Password = string.Empty;
        EmailAddress = string.Empty;
        ClientToken = string.Empty;
        Nide8AuthPath = string.Empty;
    }
    public bool GetToken()
    {
        try
        {
            var content = "{\r" +
    $"    \"username\": \"{EmailAddress}\",\r\n" +
    $"    \"password\": \"{Password}\"\r\n" +
    "}";
            HttpWebRequest request3 = (HttpWebRequest)WebRequest.Create($"https://auth.mc-user.com:233/{ServerID}/authserver/authenticate");
            request3.Method = "POST";
            byte[] byteArray3 = Encoding.UTF8.GetBytes(content);
            request3.ContentLength = byteArray3.Length;
            request3.ContentType = "application/json";
            request3.Accept = "application/json";

            Stream dataStream3 = request3.GetRequestStream();
            dataStream3.Write(byteArray3, 0, byteArray3.Length);
            dataStream3.Close();
            WebResponse response3 = request3.GetResponse();
            dataStream3 = response3.GetResponseStream();
            StreamReader reader3 = new StreamReader(dataStream3);
            var responseFromServer = reader3.ReadToEnd();
            dataStream3.Close();
            reader3.Close();
            var jOb = JObject.Parse(responseFromServer);
            EmailAddress = EmailAddress;
            AccessToken = jOb["accessToken"].ToString();
            ClientToken = jOb["clientToken"].ToString();
            UserName = jOb["selectedProfile"]["name"].ToString();
            Uuid = jOb["selectedProfile"]["id"].ToString();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static UnitedPassportAccount GetAccountWithToken(string serverID, string emailAddress, string password)
    {
        var content = "{\r" +
            $"    \"username\": \"{emailAddress}\",\r\n" +
            $"    \"password\": \"{password}\"\r\n" +
            "}";
        HttpWebRequest request3 = (HttpWebRequest)WebRequest.Create($"https://auth.mc-user.com:233/{serverID}/authserver/authenticate");
        request3.Method = "POST";
        byte[] byteArray3 = Encoding.UTF8.GetBytes(content);
        request3.ContentLength = byteArray3.Length;
        request3.ContentType = "application/json";
        request3.Accept = "application/json";

        Stream dataStream3 = request3.GetRequestStream();
        dataStream3.Write(byteArray3, 0, byteArray3.Length);
        dataStream3.Close();
        WebResponse response3 = request3.GetResponse();
        dataStream3 = response3.GetResponseStream();
        StreamReader reader3 = new StreamReader(dataStream3);
        var responseFromServer = reader3.ReadToEnd();
        dataStream3.Close();
        reader3.Close();
        var jOb = JObject.Parse(responseFromServer);
        var result = new UnitedPassportAccount()
        {
            EmailAddress = emailAddress,
            AccessToken = jOb["accessToken"].ToString(),
            ClientToken = jOb["clientToken"].ToString(),
            ServerID = serverID
        };
        return result;
    }
    public bool SingUp(out string errorInfo)
    {
        errorInfo = string.Empty;
        if (string.IsNullOrEmpty(Nide8AuthPath) || string.IsNullOrEmpty(ServerID))
        {
            errorInfo = "请先设置 Nide8Auth 的路径 和 ServerID";
            return false;
        }
        if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrEmpty(ClientToken))
        {
            errorInfo = "没有 Token";
            return false;
        }
        else
        {
            if (true)
            {
                try
                {
                    var content = "{\r" +
$"    \"accessToken\": \"{AccessToken}\",\r\n" +
$"    \"clientToken\": \"{ClientToken}\"\r\n" +
"}";
                    HttpWebRequest request3 = (HttpWebRequest)WebRequest.Create($"https://auth.mc-user.com:233/{ServerID}/authserver/refresh");
                    request3.Method = "POST";
                    byte[] byteArray3 = Encoding.UTF8.GetBytes(content);
                    request3.ContentLength = byteArray3.Length;
                    request3.ContentType = "application/json";
                    request3.Accept = "application/json";

                    Stream dataStream3 = request3.GetRequestStream();
                    dataStream3.Write(byteArray3, 0, byteArray3.Length);
                    dataStream3.Close();
                    WebResponse response3 = request3.GetResponse();
                    dataStream3 = response3.GetResponseStream();
                    StreamReader reader3 = new StreamReader(dataStream3);
                    var responseFromServer = reader3.ReadToEnd();
                    dataStream3.Close();
                    reader3.Close();
                    response3.Close();
                    var jOb = JObject.Parse(responseFromServer);
                    UserName = jOb["selectedProfile"]["name"].ToString();
                    Uuid = jOb["selectedProfile"]["id"].ToString();
                    AccessToken = jOb["accessToken"].ToString();
                    ClientToken = jOb["clientToken"].ToString();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }

    public bool Valid()
    {
        var content = "{\r" +
    $"    \"accessToken\": \"{AccessToken}\",\r\n" +
    $"    \"clientToken\": \"{ClientToken}\"\r\n" +
    "}";
        HttpWebRequest request3 = (HttpWebRequest)WebRequest.Create(
            $"https://auth.mc-user.com:233/{ServerID}/authserver/validate");
        request3.Method = "POST";
        byte[] byteArray3 = Encoding.UTF8.GetBytes(content);
        request3.ContentLength = byteArray3.Length;
        request3.ContentType = "application/json";
        request3.Accept = "application/json";

        Stream dataStream3 = request3.GetRequestStream();
        dataStream3.Write(byteArray3, 0, byteArray3.Length);
        dataStream3.Close();
        try
        {
            WebResponse response3 = request3.GetResponse();
            response3.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool SingOutAllTokens(string serverID, string emailAddress, string password, out string errorInfo)
    {
        errorInfo = string.Empty;
        var content = "{\r" +
            $"    \"username\": \"{emailAddress}\",\r\n" +
            $"    \"password\": \"{password}\"\r\n" +
            "}";
        HttpWebRequest request3 = (HttpWebRequest)WebRequest.Create($"https://auth.mc-user.com:233/{serverID}/authserver/signout");
        request3.Method = "POST";
        byte[] byteArray3 = Encoding.UTF8.GetBytes(content);
        request3.ContentLength = byteArray3.Length;
        request3.ContentType = "application/json";
        request3.Accept = "application/json";

        Stream dataStream3 = request3.GetRequestStream();
        dataStream3.Write(byteArray3, 0, byteArray3.Length);
        dataStream3.Close();
        try
        {
            WebResponse response3 = request3.GetResponse();
            response3.Close();
            return true;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("403"))
            {
                errorInfo = "操作失败";
            }
            return false;
        }
    }

    public bool RefreshToken()
    {
        {
            var content = "{\r" +
        $"    \"accessToken\": \"{AccessToken}\",\r\n" +
        $"    \"clientToken\": \"{ClientToken}\"\r\n" +
        "}";
            HttpWebRequest request3 = (HttpWebRequest)WebRequest.Create(
                $"https://auth.mc-user.com:233/{ServerID}/authserver/refresh");
            request3.Method = "POST";
            byte[] byteArray3 = Encoding.UTF8.GetBytes(content);
            request3.ContentLength = byteArray3.Length;
            request3.ContentType = "application/json";
            request3.Accept = "application/json";

            Stream dataStream3 = request3.GetRequestStream();
            dataStream3.Write(byteArray3, 0, byteArray3.Length);
            dataStream3.Close();
            try
            {
                WebResponse response3 = request3.GetResponse();
                response3.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
