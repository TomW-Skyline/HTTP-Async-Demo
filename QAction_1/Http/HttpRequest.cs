namespace QAction_1.Http
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Threading.Tasks;

	using Skyline.DataMiner.Scripting;

	public struct HttpRequest
	{
		private readonly TaskCompletionSource<HttpResponse> _tcs;

		public HttpRequest(string url)
		{
			Url = url;

			_tcs = new TaskCompletionSource<HttpResponse>();
		}

		public Task<HttpResponse> Task => _tcs.Task;

		public string Url { get; }

		public TaskAwaiter<HttpResponse> GetAwaiter()
		{
			return Task.GetAwaiter();
		}

		internal void SetResponse(SLProtocolExt protocol, HttpResponse response)
		{
			_tcs.SetResult(response);
		}

		internal void SetException(SLProtocolExt protocol, Exception exception)
		{
			_tcs.SetException(exception);
		}
	}
}