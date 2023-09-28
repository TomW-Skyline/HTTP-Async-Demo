namespace QAction_1.Http
{
	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;

	public struct HttpResponse
	{
		public HttpResponse(SLProtocolExt protocol, HttpRequest request, string resultCode, string response)
		{
			Protocol = protocol;
			Request = request;
			ResultCode = resultCode;
			Status = HttpStatus.Parse(resultCode);
			Response = response;
		}

		public SLProtocolExt Protocol { get; }

		public HttpRequest Request { get; }

		public string ResultCode { get; }

		public HttpStatus Status { get; }

		public string Response { get; }

		public bool IsSuccess => Status.IsSuccess;

		public void ThrowIfNotSuccessful()
		{
			if (!IsSuccess)
			{
				throw new HttpException(ResultCode, Status, Response);
			}
		}

		public string GetSuccessResponse()
		{
			ThrowIfNotSuccessful();

			return Response;
		}

		public T ContentAs<T>()
		{
			return JsonConvert.DeserializeObject<T>(GetSuccessResponse());
		}
	}
}