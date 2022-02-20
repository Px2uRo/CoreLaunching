using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.ObjectTemplates
{
    public class MSAuthResponse
    {
            [JsonProperty("token_type")]
            public string token_type { get; set; }

            [JsonProperty("expires_in")]
            public string expires_in { get; set; }

            [JsonProperty("scope")]
            public string scope { get; set; }

            [JsonProperty("access_token")]
            public string access_token { get; set; }

            [JsonProperty("refresh_token")]
            public string refresh_token { get; set; }

            [JsonProperty("user_id")]
            public string user_id { get; set; }

            [JsonProperty("foci")]
            public string foci { get; set; }
    }

    class XBLResponse
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

    class DisplayClaims
    {
        [JsonProperty("xui")]
        public List<Xui> xui { get; set; }

    }

    class Xui
    {
        [JsonProperty("uhs")]
        public string uhs { get; set; }
    }

    public class login_with_xboxResponse
    {
            [JsonProperty("username")]
            public string username { get; set; }

            [JsonProperty("access_token")]
            public string access_token { get; set; }

            [JsonProperty("token_type")]
            public string token_type { get; set; }

            [JsonProperty("expires_in")]
            public string expires_in { get; set; }

    }

    public class Ownership
    {
        [JsonProperty("items")]
        public List<Items> items { get; set; }

        [JsonProperty("signature")]
        public string signature { get; set; }

        [JsonProperty("keyId")]
        public string keyId { get; set; }

    }

    public class Items
    {

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("signature")]
        public string signature { get; set; }

    }

    public class PlayerInfo
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("skins")]
        public List<Skins> skins { get; set; }

        [JsonProperty("capes")]
        public List<Capes> capes { get; set; }

        public string access_token { get; set; }
    }

    public class Skins
    {

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("state")]
        public string state { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("variant")]
        public string variant { get; set; }

    }

    public class Capes
    {

    }
}
