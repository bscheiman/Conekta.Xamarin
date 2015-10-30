using System;

namespace Conekta.Xamarin.Droid {
	public class ConektaTokenizer : BaseConektaTokenizer {
		public ConektaTokenizer(string publicKey) : base(publicKey, RuntimePlatform.iOS) {
		}

		public override string GetFingerprint() {
			throw new NotImplementedException();
		}

		public override string GetUserAgent() {
			throw new NotImplementedException();
		}

		public override void CollectDevice() {
			throw new NotImplementedException();
		}
	}
}

