using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.MicrosoftAuth
{
    public class MSAuthResponse
    {
            [JsonProperty("token_type")]
            public string Token_type { get; set; }

            [JsonProperty("expires_in")]
            public string Expires_in { get; set; }

            [JsonProperty("scope")]
            public string Scope { get; set; }

            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonProperty("user_id")]
            public string UserId { get; set; }

            [JsonProperty("foci")]
            public string Foci { get; set; }
    }

    public class XBLResponse
    {
        [JsonProperty("IssueInstant")]
        public string IssueInstant { get; set; }

        [JsonProperty("NotAfter")]
        public string NotAfter { get; set; }

        [JsonProperty("Token")]
        public string Token { get; set; }

        [JsonProperty("DisplayClaims")]
        public DisplayClaims DisplayClaims { get; set; }

    }

    public class DisplayClaims
    {
        [JsonProperty("xui")]
        public List<Xui> Xui { get; set; }

    }

    public class Xui
    {
        [JsonProperty("uhs")]
        public string Uhs { get; set; }
    }

    public class Login_With_XboxResponse
    {
            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public string ExpiresIn { get; set; }

    }

    public class Ownership
    {
        [JsonProperty("items")]
        public List<Items> Items { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("keyId")]
        public string KeyId { get; set; }

    }

    public class Items
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

    }

    public class MSAPlayerInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("skins")]
        public List<Skin> Skins { get; set; }

        [JsonProperty("capes")]
        public List<Capes> Capes { get; set; }

        public string AccessToken { get; set; }
    }
    public class MSAPlayerInfoWithRefreshToken
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("skins")]
        public List<Skin> Skins { get; set; }

        [JsonProperty("capes")]
        public List<Capes> Capes { get; set; }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool HasError { get; set; }
        public string ErrorInfo { get; set; }
        public string ErrorCode { get; set; }
    }

    public class Skin
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("variant")]
        public string Variant { get; set; }

    }

    public class Capes
    {

    }
}
