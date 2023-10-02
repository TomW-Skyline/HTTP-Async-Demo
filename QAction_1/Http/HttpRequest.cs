namespace QAction_1.Http
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using System.Threading.Tasks;

	using Skyline.DataMiner.Scripting;

	public class HttpRequest
	{
		private readonly TaskCompletionSource<(SLProtocolExt, HttpResponse)> _tcs;

		protected HttpRequest(HttpVerb verb, string url)
		{
			Verb = verb;
			Url = url ?? throw new ArgumentNullException(nameof(url));

			_tcs = new TaskCompletionSource<(SLProtocolExt, HttpResponse)>();
		}

		public Task<(SLProtocolExt, HttpResponse)> Task => _tcs.Task;

		public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

		public HttpVerb Verb { get; }

		public string Url { get; }

		public string Data { get; set; }

		public TaskAwaiter<(SLProtocolExt protocol, HttpResponse response)> GetAwaiter()
		{
			return Task.GetAwaiter();
		}

		internal void SetResponse(SLProtocolExt protocol, HttpResponse response)
		{
			_tcs.SetResult((protocol, response));
		}
		 
		internal void SetException(Exception exception)
		{
			_tcs.SetException(exception);
		}
	}

	public class HttpGetRequest : HttpRequest
	{
		public HttpGetRequest(string url) : base(HttpVerb.Get, url)
		{
		}
	}

	public class HttpPostRequest : HttpRequest
	{
		public HttpPostRequest(string url, string data) : base(HttpVerb.Post, url)
		{
			Data = data;
		}
	}

	public class HttpPutRequest : HttpRequest
	{
		public HttpPutRequest(string url, string data) : base(HttpVerb.Put, url)
		{
			Data = data;
		}
	}

	public class HttpDeleteRequest : HttpRequest
	{
		public HttpDeleteRequest(string url) : base(HttpVerb.Delete, url)
		{
		}
	}
}