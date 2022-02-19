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
}
