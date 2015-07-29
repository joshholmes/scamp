using Newtonsoft.Json;

namespace SCAMP.Azure
{
    public class AccessToken
    {
        public string Resource { get; set; }

        [JsonProperty("access_token")]
        public string Token { get; set; }
    }
}
