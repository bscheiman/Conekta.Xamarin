using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Conekta.Xamarin {
	public abstract class BaseConektaTokenizer {
		internal Version CurrentVersion = new Version(0, 5, 0);
		public RuntimePlatform Platform { get; private set; }
		public abstract string GetFingerprint();
		public abstract string GetUserAgent();
		public abstract void CollectDevice();
		private string PublicKey { get; set; }

		protected BaseConektaTokenizer(string publicKey, RuntimePlatform platform) {
			if (string.IsNullOrEmpty(publicKey))
				throw new ArgumentException("publicKey");

			PublicKey = publicKey;
			Platform = platform;
		}

		public async Task<string> CreateTokenAsync(string cardNumber, string name, string cvc, int year, int month) {
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

			return await CreateTokenAsync(cardNumber, name, cvc, new DateTime(year, month, 1));
		}

		public async Task<string> CreateTokenAsync(Card card) {
			try {
				if (string.IsNullOrEmpty(card.Number))
					throw new ArgumentNullException("cardNumber");

				if (string.IsNullOrEmpty(card.Name))
					throw new ArgumentNullException("name");

				if (string.IsNullOrEmpty(card.Cvc))
					throw new ArgumentNullException("cvc");

				using (var client = new HttpClient()) {
					var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:", PublicKey)));
					var version = string.Format("application/vnd.conekta-v{0}.{1}.{2}+json",
					                            CurrentVersion.Major,
					                            CurrentVersion.Minor,
					                            CurrentVersion.Build);

					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(version));
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
					client.DefaultRequestHeaders.Add("Conekta-Client-User-Agent", string.Format(@"{{""agent"": ""{0}""}}",GetUserAgent()));

					var req = new HttpRequestMessage(HttpMethod.Post, "https://api.conekta.io/tokens") {
						Content =
							new StringContent(
							string.Format(
								@"{{""card"":{{""name"":""{0}"",""number"":{1},""cvc"":{2},""exp_month"":{3},""exp_year"":{4}}}}}", card.Name,
								card.Number, card.Cvc, card.Expiration.Month, card.Expiration.Year), Encoding.UTF8, "application/json")
					};

					string str = await (await client.SendAsync(req)).Content.ReadAsStringAsync();

					return Regex.Match(str, @"""id"":""(?<Id>[^""]+)""").Groups["Id"].Value;
				}
			} catch {
				return null;
			}
		}

		public Task<string> CreateTokenAsync(string cardNumber, string name, string cvc, DateTime expiration) {
			return CreateTokenAsync(new Card {
				Number = cardNumber,
				Name = name,
				Cvc = cvc,
				Expiration = expiration
			});
		}
	}
}

