using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluentlyHttpClient
{
	/// <summary>
	/// FluentHttpRequest Extensions
	/// </summary>
	public static class FluentHttpRequestExtensions
	{
		/// <summary>
		/// Call FluentHttpRequest's FluentHttpClient SendAsync method.
		/// </summary>
		/// <typeparam name="T">Return type</typeparam>
		/// <param name="request">FluentHttpRequest</param>
		/// <returns>T</returns>
		public static async Task<T> SendAsync<T>(this FluentHttpRequest request)
		{
			var response = await request.FluentHttpClient.SendAsync<T>(request);
			return response.Data;
		}

		/// <summary>
		/// Build FluentHttpRequestBuilder and  call returned FluentHttpRequest's FluentHttpClient SendAsync method.
		/// </summary>
		/// <typeparam name="T">Return type</typeparam>
		/// <param name="builder">FluentHttpRequest</param>
		/// <returns>T</returns>
		public static async Task<T> SendAsync<T>(this FluentHttpRequestBuilder builder)
		{
			var request = builder.Build();
			return await SendAsync<T>(request);
		}
	}
}
