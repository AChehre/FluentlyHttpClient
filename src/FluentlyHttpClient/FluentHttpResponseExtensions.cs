using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FluentlyHttpClient
{
	/// <summary>
	///     FluentHttpResponse Extensions
	/// </summary>
	public static class FluentHttpResponseExtensions
	{
		/// <summary>
		///     Read FluentHttpResponse as Stream
		/// </summary>
		/// <typeparam name="T">Response content type</typeparam>
		/// <param name="response">T</param>
		/// <returns></returns>
		public static async Task<T> As<T>(this FluentHttpResponse response)
		{
			var streamContent = await response.Message.Content.ReadAsStreamAsync().ConfigureAwait(false);
			return DeserializeJsonFromStream<T>(streamContent);
		}


		private static T DeserializeJsonFromStream<T>(Stream stream)
		{
			if (stream == null || stream.CanRead == false)
			{
				return default;
			}

			using (var sr = new StreamReader(stream))
			using (var jtr = new JsonTextReader(sr))
			{
				var js = new JsonSerializer();
				var searchResult = js.Deserialize<T>(jtr);
				return searchResult;
			}
		}

		/// <summary>
		///     Read FluentHttpResponse as String
		/// </summary>
		/// <param name="response">String content</param>
		/// <returns></returns>
		public static async Task<string> AsString(this FluentHttpResponse response)
		{
			return await response.Message.Content.ReadAsStringAsync().ConfigureAwait(false);
		}

		/// <summary>
		///     Read FluentHttpResponse as ByteArray
		/// </summary>
		/// <param name="response">ByteArray content</param>
		/// <returns></returns>
		public static async Task<byte[]> AsByteArray(this FluentHttpResponse response)
		{
			return await response.Message.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
		}

		/// <summary>
		///     Read FluentHttpResponse as Stream
		/// </summary>
		/// <param name="response">Stream content</param>
		/// <returns></returns>
		public static async Task<Stream> AsStream(this FluentHttpResponse response)
		{
			var stream = await response.Message.Content.ReadAsStreamAsync().ConfigureAwait(false);
			stream.Position = 0;
			return stream;
		}
	}
}