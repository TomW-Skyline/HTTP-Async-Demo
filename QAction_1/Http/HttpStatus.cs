namespace QAction_1.Http
{
	using System;
	using System.IO;
	using System.Text.RegularExpressions;

	public struct HttpStatus
	{
		public string Version { get; private set; }

		public int Code { get; private set; }

		public string Reason { get; private set; }

		public bool IsSuccess => Code >= 200 && Code < 300;

		public static HttpStatus Parse(string statusLine)
		{
			var match = Regex.Match(statusLine, @"^(?<version>.+)\s(?<code>\d{3})(\s(?<reason>.*))?$");

			if (!match.Success)
			{
				throw new InvalidDataException("Invalid HTTP status line");
			}

			var httpVersion = match.Groups["version"].Value;
			var statusCode = Int32.Parse(match.Groups["code"].Value);
			var reasonPhrase = match.Groups["reason"].Success ? match.Groups["reason"].Value : String.Empty;

			return new HttpStatus
			{
				Version = httpVersion,
				Code = statusCode,
				Reason = reasonPhrase,
			};
		}

		public override string ToString()
		{
			return $"{Version} {Code} {Reason}";
		}
	}
}