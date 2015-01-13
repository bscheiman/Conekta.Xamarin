#region
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#endregion

namespace Conekta.Xamarin {
    public class ConektaTokenizer {
        public string PublicKey { get; internal set; }

        public ConektaTokenizer(string publicKey) {
            if (string.IsNullOrEmpty(publicKey))
                throw new ArgumentException("publicKey");

            PublicKey = publicKey;
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

            string platform = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Contains(".config") ? "android" : "ios";

            using (var wc = new WebClient()) {
                wc.Headers["Accept"] = "application/vnd.conekta-v0.3.0+json";
                wc.Headers["Content-Type"] = "application/json";

                if (platform == "android")
                    wc.Headers["Conekta-Client-User-Agent"] = @"{""agent"": ""Conekta Android SDK""}";
                else if (platform == "ios")
                    wc.Headers["Conekta-Client-User-Agent"] = @"{""agent"": ""Conekta iOS SDK""}";

                wc.Credentials = new NetworkCredential(PublicKey, string.Empty);

                string str =
                    await
                        wc.UploadStringTaskAsync(new Uri("https://api.conekta.io/tokens"), "POST",
                            string.Format(
                                @"{{""card"":{{""name"":""{0}"",""number"":{1},""cvc"":{2},""exp_month"":{3},""exp_year"":{4}}}}}", name,
                                cardNumber, cvc, expiry.Month, expiry.Year));

                // Ye nasty regex hack para no depender de JSON serializers

                return Regex.Match(str, @"""id"":""(?<Id>[^""]+)""").Groups["Id"].Value;
            }
        }
    }
}