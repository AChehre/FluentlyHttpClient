using System;
using System.Collections.Generic;
using System.Text;

namespace FluentlyHttpClient.Constants
{
	/// <summary>
	/// HTTP header types such as User-Agent, Authorization, etc...
	/// </summary>
	public static class HeaderTypes
	{
		/// <summary>
		/// Gets the Accept header name.
		/// </summary>
		public const string Accept = "Accept";

		/// <summary>
		/// Gets the Authorization header name.
		/// </summary>
		public const string Authorization = "Authorization";

		/// <summary>
		/// Gets the X-Forwarded-For header name.
		/// </summary>
		public const string XForwardedFor = "X-Forwarded-For";

		/// <summary>
		/// Gets the User-Agent header name.
		/// </summary>
		public const string UserAgent = "User-Agent";
	}

}
