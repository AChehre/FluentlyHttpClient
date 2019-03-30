using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace FluentlyHttpClient
{
	/// <summary>
	///     Delegate which is mainly used by Middleware.
	/// </summary>
	/// <param name="request">HTTP request to send.</param>
	/// <returns>Returns async response.</returns>
	public delegate Task<FluentHttpResponse> FluentHttpRequestDelegate(FluentHttpRequest request);

	/// <summary>
	///     Fluent HTTP request, which wraps the <see cref="HttpRequestMessage" /> and add additional features.
	/// </summary>
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class FluentHttpRequest : IFluentHttpMessageState
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public FluentHttpRequest(HttpRequestMessage message, IDictionary<object, object> items = null)
		{
			Message = message;
			Items = items == null
				? new Dictionary<object, object>()
				: new Dictionary<object, object>(items);
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public FluentHttpRequest(IFluentHttpClient fluentHttpClient, HttpRequestMessage message,
			IDictionary<object, object> items = null)
		{
			FluentHttpClient = fluentHttpClient;
			Message = message;
			Items = items == null
				? new Dictionary<object, object>()
				: new Dictionary<object, object>(items);
		}
		/// <summary>
		/// FluentHttpClient that start current request.
		/// </summary>
		public IFluentHttpClient FluentHttpClient { get; }
		private string DebuggerDisplay => $"[{Method}] '{Uri}'";

		/// <summary>
		///     Gets the underlying HTTP request message.
		/// </summary>
		public HttpRequestMessage Message { get; }

		/// <summary>
		/// Gets the request builder which is responsible for this request message.
		/// </summary>
		public FluentHttpRequestBuilder Builder { get; }

		/// <summary>
		/// Gets or sets the <see cref="HttpMethod"/> for the HTTP request.
		/// </summary>
		public HttpMethod Method
		{
			get => Message.Method;
			set => Message.Method = value;
		}

		/// <summary>
		///     Gets or sets the <see cref="System.Uri" /> for the HTTP request.
		/// </summary>
		public Uri Uri
		{
			get => Message.RequestUri;
			set => Message.RequestUri = value;
		}

		/// <summary>
		///     Gets the collection of HTTP request headers.
		/// </summary>
		public HttpRequestHeaders Headers => Message.Headers;

		/// <summary>
		///     Determine whether has success status otherwise it will throw or not.
		///		This property is overriding FluentHttpClients HasSuccessStatusOrThrow behavior.
		/// </summary>
		public bool? HasSuccessStatusOrThrow { get; set; }

		/// <summary>
		///     Cancellation token to cancel operation.
		/// </summary>
		public CancellationToken CancellationToken { get; set; }

		/// <summary>
		///     Formatters to be used for content negotiation for "Accept" and also sending formats. e.g. (JSON, XML)
		/// </summary>
		public MediaTypeFormatterCollection Formatters { get; set; }

		/// <inheritdoc />
		public IDictionary<object, object> Items { get; protected set; }


		/// <summary>
		///     Gets readable request info as string.
		/// </summary>
		public FluentHttpRequest(FluentHttpRequestBuilder builder, IFluentHttpClient fluentHttpClient, HttpRequestMessage message, IDictionary<object, object> items = null)
		{
			Message = message;
			Builder = builder;
			FluentHttpClient = fluentHttpClient;
			Items = items == null
				? new Dictionary<object, object>()
				: new Dictionary<object, object>(items);
		}
	}
}