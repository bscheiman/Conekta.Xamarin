using Newtonsoft.Json;

namespace Conekta.Xamarin.PCL.Extensions {
	public static class StringExtensions {
		public static T FromJson<T>(this string str) {
			return JsonConvert.DeserializeObject<T>(str);
		}
	}
}

