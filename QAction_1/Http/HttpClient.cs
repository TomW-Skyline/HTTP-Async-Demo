namespace QAction_1.Http
{
	using System;
	using System.Collections.Concurrent;
	using System.IO;

	using Skyline.DataMiner.Scripting;

	public class HttpClient
	{
		private readonly ConcurrentQueue<HttpRequest> _queue = new ConcurrentQueue<HttpRequest>();
		private bool _requestOngoing;

		public HttpRequest Get(SLProtocolExt protocol, string url)
		{
			var request = new HttpRequest(url);
			_queue.Enqueue(request);

			if (!_requestOngoing)
			{
				SendNextRequest(protocol);
			}

			return request;
		}

		public void RegisterResponse(SLProtocolExt protocol, string resultCode, string response)
		{
			if (_queue.TryDequeue(out var lastRequest))
			{
				if (resultCode.StartsWith("HTTP/1.1 200"))
				{
					lastRequest.SetResponse(protocol, new HttpResponse(lastRequest, resultCode, response));
				}
				else
				{
					lastRequest.SetException(protocol, new InvalidDataException("HTTP Status code: " + resultCode));
				}

				_requestOngoing = false;
			}
			else
			{
				throw new InvalidOperationException("No ongoing request");
			}

			SendNextRequest(protocol);
		}

		private void SendNextRequest(SLProtocolExt protocol)
		{
			if (!_queue.TryPeek(out var request))
			{
				return;
			}

			protocol.Httprequesturi = request.Url;
			_requestOngoing = true;
		}
	}
}