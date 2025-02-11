using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace WebApplication1
{
    public class ReCaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        public ReCaptchaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _secretKey = "6LemJdMqAAAAAE2pgrqKJlRytugZqzdaXWWMmI5V";
        }

        public async Task<bool> VerifyToken(string token)
        {
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("secret", _secretKey),
            new KeyValuePair<string, string>("response", token)
        });

            var response = await _httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(responseString);

            return reCaptchaResponse.Success && reCaptchaResponse.Score >= 0.5; // Adjust score threshold as needed
        }

        private class ReCaptchaResponse
        {
            public bool Success { get; set; }
            public double Score { get; set; }
            public string Action { get; set; }
        }
    }
}
