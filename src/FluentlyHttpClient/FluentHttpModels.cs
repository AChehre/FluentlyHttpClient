﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FluentlyHttpClient
{
	public delegate Task<IFluentHttpResponse> FluentHttpRequestDelegate(FluentHttpRequest request);

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class FluentHttpRequest
	{
		private string DebuggerDisplay => $"[{Method}] '{Url}'";

		public HttpRequestMessage RawRequest { get; }

		public HttpMethod Method => RawRequest.Method;

		public Uri Url => RawRequest.RequestUri;

		public HttpRequestHeaders Headers => RawRequest.Headers;

		// todo: remove?
		public object Data { get; set; }

		public FluentHttpRequest(HttpRequestMessage rawRequest)
		{
			RawRequest = rawRequest;
		}

		public override string ToString() => $"{DebuggerDisplay}";
	}

	
	public interface IFluentHttpResponse
	{
		HttpStatusCode StatusCode { get; }
		bool IsSuccessStatusCode { get; }
		void EnsureSuccessStatusCode();
		string ReasonPhrase { get; }
		HttpResponseHeaders Headers { get; }
		IDictionary<object, object> Items { get; set; }
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class FluentHttpResponse<T> : IFluentHttpResponse
	{
		private string DebuggerDisplay => $"[{(int)StatusCode}] '{ReasonPhrase}', Request: {{ [{RawResponse.RequestMessage.Method}] '{RawResponse.RequestMessage.RequestUri}' }}";

		public HttpResponseMessage RawResponse { get; }

		public FluentHttpResponse(HttpResponseMessage rawResponse)
		{
			RawResponse = rawResponse;
		}

		public T Data { get; set; }

		public HttpStatusCode StatusCode => RawResponse.StatusCode;
		public bool IsSuccessStatusCode => RawResponse.IsSuccessStatusCode;
		public void EnsureSuccessStatusCode() => RawResponse.EnsureSuccessStatusCode();
		public string ReasonPhrase => RawResponse.ReasonPhrase;
		public HttpResponseHeaders Headers => RawResponse.Headers;

		/// <summary>
		/// Gets or sets a key/value collection that can be used to share data within the scope of request/response.
		/// </summary>
		public IDictionary<object, object> Items { get; set; } = new Dictionary<object, object>();

		public override string ToString() => $"{DebuggerDisplay}";
	}

	
}