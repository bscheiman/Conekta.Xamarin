#region
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#endregion

namespace Conekta.Xamarin {
    public class ConektaTokenizer {
        public RuntimePlatform Platform { get; set; }
        public string PublicKey { get; internal set; }

        public ConektaTokenizer(string publicKey, RuntimePlatform platform) {
            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException("publicKey");

            PublicKey = publicKey;
            Platform = platform;
        }

        public async Task<string> GetTokenAsync(string cardNumber, string name, string cvc, int year, int month) {
            if (year <= 0)
                throw new ArgumentOutOfRangeException("year");

            if (month <= 0 || month > 12)
                throw new ArgumentOutOfRangeException("month");

            if (string.IsNullOrEmpty(cardNumber))
                throw new ArgumentNullException("cardNumber");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(cvc))
                throw new ArgumentNullException("cvc");

            return await GetTokenAsync(cardNumber, name, cvc, new DateTime(year, month, 1));
        }

        public async Task<string> GetTokenAsync(string cardNumber, string name, string cvc, DateTime expiry) {
            if (string.IsNullOrEmpty(cardNumber))
                throw new ArgumentNullException("cardNumber");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(cvc))
                throw new ArgumentNullException("cvc");

            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:", PublicKey))));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.conekta-v0.3.0+json"));

                if (Platform == RuntimePlatform.Android)
                    client.DefaultRequestHeaders.Add("Conekta-Client-User-Agent", @"{""agent"": ""Conekta Android SDK""}");
                else if (Platform == RuntimePlatform.iOS)
                    client.DefaultRequestHeaders.Add("Conekta-Client-User-Agent", @"{""agent"": ""Conekta iOS SDK""}");

                var req = new HttpRequestMessage(HttpMethod.Post, "https://api.conekta.io/tokens") {
                    Content =
                        new StringContent(
                            string.Format(
                                @"{{""card"":{{""name"":""{0}"",""number"":{1},""cvc"":{2},""exp_month"":{3},""exp_year"":{4}}}}}", name,
                                cardNumber, cvc, expiry.Month, expiry.Year), Encoding.UTF8, "application/json")
                };

                string str = await (await client.SendAsync(req)).Content.ReadAsStringAsync();

                // Ye nasty regex hack para no depender de JSON serializers

                return Regex.Match(str, @"""id"":""(?<Id>[^""]+)""").Groups["Id"].Value;
            }
        }
    }
}