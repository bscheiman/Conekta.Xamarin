using System;
using Conekta.Xamarin;
using Conekta.DeviceCollector;
using UIKit;
using System.Threading.Tasks;

namespace Conekta.Xamarin.iOS {
	public class ConektaTokenizer : BaseConektaTokenizer {
		private DeviceDelegate Delegate { get; set; }
		private TaskCompletionSource<string> Task { get; set; }

		public ConektaTokenizer(string publicKey) : base(publicKey, RuntimePlatform.iOS) {
		}

		public override string GetFingerprint() {
			return UIDevice.CurrentDevice.IdentifierForVendor.AsString().Replace("-", "");
		}

		public override string GetUserAgent() {
			return "Conekta.Xamarin iOS";
		}

		public override void CollectDevice() {
			throw new NotImplementedException();
		}
	}
}

