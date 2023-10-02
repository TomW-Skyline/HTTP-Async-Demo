namespace QAction_1.Http
{
	using System;
	using System.Collections.Generic;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;

	public class HttpClient
	{
		private readonly Action<SLProtocolExt, HttpRequest> _sendRequestAction;

		private readonly Queue<HttpRequest> _queue = new Queue<HttpRequest>();
		private bool _requestOngoing;

		public HttpClient(Action<SLProtocolExt, HttpRequest> sendRequestAction)
		{
			_sendRequestAction = sendRequestAction ?? throw new ArgumentNullException(nameof(sendRequestAction));
		}

		public void Get(SLProtocolExt protocol, HttpGetRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			Request(protocol, request);
		}

		public HttpGetRequest Get(SLProtocolExt protocol, string url)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));

			var request = new HttpGetRequest(url);
			Get(protocol, request);

			return request;
		}

		public void Post(SLProtocolExt protocol, HttpPostRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			Request(protocol, request);
		}

		public HttpPostRequest Post(SLProtocolExt protocol, string url, string data)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			var request = new HttpPostRequest(url, data);
			Post(protocol, request);

			return request;
		}

		public HttpPostRequest PostJson<T>(SLProtocolExt protocol, string url, T value)
		{
			string data = JsonConvert.SerializeObject(value);
			return Post(protocol, url, data);
		}

		public void Put(SLProtocolExt protocol, HttpPutRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			Request(protocol, request);
		}

		public HttpPutRequest Put(SLProtocolExt protocol, string url, string data)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			var request = new HttpPutRequest(url, data);
			Put(protocol, request);

			return request;
		}

		public HttpPutRequest PutJson<T>(SLProtocolExt protocol, string url, T value)
		{
			string data = JsonConvert.SerializeObject(value);
			return Put(protocol, url, data);
		}

		public void Delete(SLProtocolExt protocol, HttpDeleteRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			Request(protocol, request);
		}

		public HttpDeleteRequest Delete(SLProtocolExt protocol, string url)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));

			var request = new HttpDeleteRequest(url);
			Delete(protocol, request);

			return request;
		}

		public void Request(SLProtocolExt protocol, HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

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

		public void RegisterResponse(SLProtocolExt protocol, HttpResponse response)
		{
			if (response == null)
			{
				throw new ArgumentNullException(nameof(response));
			}

			lock (_queue)
			{
				try
				{
					var request = _queue.Dequeue();
					response.SetRequest(request);
					request.SetResponse(protocol, response);
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

		private void SendNextRequest(SLProtocolExt protocol)
		{
			var request = _queue.Peek();
			_sendRequestAction(protocol, request);
		}
	}
}