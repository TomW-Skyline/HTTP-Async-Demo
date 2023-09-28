namespace QAction_1.Http
{
	using System;
	using System.Collections.Generic;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;

	public class HttpClient
	{
		private readonly string _uriBase;
		private readonly IHttpActions _actions;

		private readonly Queue<HttpRequest> _queue = new Queue<HttpRequest>();
		private bool _requestOngoing;

		public HttpClient(string uriBase, IHttpActions actions)
		{
			_uriBase = uriBase ?? throw new ArgumentNullException(nameof(uriBase));
			_actions = actions ?? throw new ArgumentNullException(nameof(actions));
		}

		public HttpGetRequest Get(SLProtocolExt protocol, string url)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));

			var request = new HttpGetRequest(url);
			NewRequest(protocol, request);

			return request;
		}

		public HttpPostRequest Post(SLProtocolExt protocol, string url, string data)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			var request = new HttpPostRequest(url, data);
			NewRequest(protocol, request);

			return request;
		}

		public HttpPostRequest PostJson<T>(SLProtocolExt protocol, string url, T value)
		{
			string data = JsonConvert.SerializeObject(value);
			return Post(protocol, url, data);
		}

		public HttpPutRequest Put(SLProtocolExt protocol, string url, string data)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			var request = new HttpPutRequest(url, data);
			NewRequest(protocol, request);

			return request;
		}

		public HttpPutRequest PutJson<T>(SLProtocolExt protocol, string url, T value)
		{
			string data = JsonConvert.SerializeObject(value);
			return Put(protocol, url, data);
		}

		public HttpDeleteRequest Delete(SLProtocolExt protocol, string url)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));

			var request = new HttpDeleteRequest(url);
			NewRequest(protocol, request);

			return request;
		}

		public void RegisterResponse(SLProtocolExt protocol, string resultCode, string response)
		{
			lock (_queue)
			{
				try
				{
					var request = _queue.Dequeue();
					request.SetResponse(new HttpResponse(protocol, request, resultCode, response));
				}
				finally
				{
					if (_queue.Count > 0)
					{
						SendNextRequest(protocol);
					}
					else
					{
						_requestOngoing = false;
					}
				}
			}
		}

		private void NewRequest(SLProtocolExt protocol, HttpRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			lock (_queue)
			{
				_queue.Enqueue(request);

				if (!_requestOngoing)
				{
					_requestOngoing = true;
					SendNextRequest(protocol);
				}
			}
		}

		private void SendNextRequest(SLProtocolExt protocol)
		{
			var request = _queue.Peek();

			var url = $"{_uriBase}{request.Url}";

			if (url.StartsWith("/"))
			{
				url = url.Substring(1);
			}

			switch (request)
			{
				case HttpGetRequest getRequest:
					_actions.Get(protocol, url, getRequest);
					break;
				case HttpPostRequest postRequest:
					_actions.Post(protocol, url, postRequest);
					break;
				case HttpPutRequest putRequest:
					_actions.Put(protocol, url, putRequest);
					break;
				case HttpDeleteRequest deleteRequest:
					_actions.Delete(protocol, url, deleteRequest);
					break;
				default:
					throw new NotSupportedException($"Unsupported type: {request.GetType().Name}");
			}
		}
	}
}