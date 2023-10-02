namespace QAction_1.Http
{
	using System.Collections.Generic;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;

	public class HttpResponse
	{
		public HttpResponse(string resultCode, string response)
		{
			ResultCode = resultCode;
			Status = HttpStatus.Parse(resultCode);
			Response = response;
		}

		public HttpRequest Request { get; private set; }

		public string ResultCode { get; }

		public HttpStatus Status { get; }

		public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

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

		internal void SetRequest(HttpRequest request)
		{
			Request = request ?? throw new System.ArgumentNullException(nameof(request));
		}
	}
}