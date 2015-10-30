using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Conekta.DeviceCollector {
	[Protocol, Model]
	[BaseType(typeof(NSObject))]
	interface DeviceCollectorSDKDelegate {
		[Export("onCollectorStart")]
		void OnCollectorStart();

		[Export("onCollectorSuccess")]
		void OnCollectorSuccess();

		[Export("onCollectorError:withError:")]
		void OnCollectorError(int errorCode, NSError error);
	}

	// @interface DeviceCollectorSDK : NSObject <UIWebViewDelegate>
	[BaseType(typeof(NSObject))]
	interface DeviceCollectorSDK : IUIWebViewDelegate {
		[Export("skipList", ArgumentSemantic.Strong)]
		NSObject[] SkipList { get; set; }

		[Export("initWithDebugOn:")]
		IntPtr Constructor(bool debugLogging);

		[Export("setCollectorUrl:")]
		void SetCollectorUrl(string url);

		[Export("setMerchantId:")]
		void SetMerchantId(string merc);

		[Export("collect:")]
		void Collect(string sessionId);

		[Export("setDelegate:")]
		void SetDelegate(DeviceCollectorSDKDelegate @delegate);
	}
}
