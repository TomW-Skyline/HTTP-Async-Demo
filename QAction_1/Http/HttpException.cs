namespace QAction_1.Http
{
	using System;

#pragma warning disable S4027 // Exceptions should provide standard constructors
	public class HttpException : Exception
#pragma warning restore S4027 // Exceptions should provide standard constructors
	{
		public HttpException(string resultCode, HttpStatus status, string response)
		{
			ResultCode = resultCode;
			Status = status;
			Response = response;
		}

		public string ResultCode { get; }

		public HttpStatus Status { get; }

		public string Response { get; }

		public override string ToString()
		{
			return $"{ResultCode}: {Response}";
		}
	}
}