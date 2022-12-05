namespace QAction_1.Http
{
	public struct HttpResponse
	{
		public HttpResponse(HttpRequest request, string resultCode, string response)
		{
			Request = request;
			ResultCode = resultCode;
			Response = response;
		}

		public HttpRequest Request { get; }

		public string ResultCode { get; }

		public string Response { get; }
	}
}