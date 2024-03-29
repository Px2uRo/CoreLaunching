﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLaunching.JsonTemplates
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
            public string Access_Token { get; set; }

            [JsonProperty("refresh_token")]
            public string Refresh_Token { get; set; }

            [JsonProperty("user_id")]
            public string User_Id { get; set; }

            [JsonProperty("foci")]
            public string Foci { get; set; }
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
        public List<Xui> Xui { get; set; }

    }

    class Xui
    {
        [JsonProperty("uhs")]
        public string Uhs { get; set; }
    }

    public class Login_With_XboxResponse
    {
            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("access_token")]
            public string Access_Token { get; set; }

            [JsonProperty("token_type")]
            public string Token_Type { get; set; }

            [JsonProperty("expires_in")]
            public string Expires_In { get; set; }

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

    public class PlayerInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("skins")]
        public List<Skins> Skins { get; set; }

        [JsonProperty("capes")]
        public List<Capes> Capes { get; set; }

        public string Access_Token { get; set; }
    }

    public class Skins
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
