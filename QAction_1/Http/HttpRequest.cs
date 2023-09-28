namespace QAction_1.Http
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Threading.Tasks;

	public class HttpRequest
	{
		private readonly TaskCompletionSource<HttpResponse> _tcs;

		protected HttpRequest(string url)
		{
			Url = url ?? throw new ArgumentNullException(nameof(url));

			_tcs = new TaskCompletionSource<HttpResponse>();
		}

		public Task<HttpResponse> Task => _tcs.Task;

		public string Url { get; }

		public TaskAwaiter<HttpResponse> GetAwaiter()
		{
			return Task.GetAwaiter();
		}

		internal void SetResponse(HttpResponse response)
		{
			_tcs.SetResult(response);
		}

		internal void SetException(Exception exception)
		{
			_tcs.SetException(exception);
		}
	}

	public class HttpGetRequest : HttpRequest
	{
		public HttpGetRequest(string url) : base(url)
		{
		}
	}

	public class HttpPostRequest : HttpRequest
	{
		public HttpPostRequest(string url, string data) : base(url)
		{
			Data = data ?? throw new ArgumentNullException(nameof(data));
		}

		public string Data { get; }
	}

	public class HttpPutRequest : HttpRequest
	{
		public HttpPutRequest(string url, string data) : base(url)
		{
			Data = data ?? throw new ArgumentNullException(nameof(data));
		}

		public string Data { get; }
	}

	public class HttpDeleteRequest : HttpRequest
	{
		public HttpDeleteRequest(string url) : base(url)
		{
		}
	}
}