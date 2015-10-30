using System.Threading.Tasks;
using Conekta.DeviceCollector;

namespace Conekta.Xamarin.iOS {

	public class DeviceDelegate : DeviceCollectorSDKDelegate {
		private TaskCompletionSource<string> Task { get; set; }

		public DeviceDelegate(TaskCompletionSource<string> task) {
			Task = task;
		}

		public override void OnCollectorError(int errorCode, Foundation.NSError error) {
			base.OnCollectorError(errorCode, error);
		}

		public override void OnCollectorStart() {
			base.OnCollectorStart();
		}

		public override void OnCollectorSuccess() {
			base.OnCollectorSuccess();
		}
	}
}
