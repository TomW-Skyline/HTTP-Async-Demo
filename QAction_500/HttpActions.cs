namespace QAction_500
{
	using QAction_1.Http;

	using Skyline.DataMiner.Scripting;

	public class HttpActions : IHttpActions
	{
		public void Get(SLProtocolExt protocol, string url, HttpGetRequest request)
		{
			protocol.Httprequesturi = url;
			protocol.CheckTrigger(510);
		}

		public void Post(SLProtocolExt protocol, string url, HttpPostRequest request)
		{
			protocol.SetParameters(new[] { Parameter.httprequesturi, Parameter.httprequestdata }, new object[] { url, request.Data });
			protocol.CheckTrigger(511);
		}

		public void Put(SLProtocolExt protocol, string url, HttpPutRequest request)
		{
			protocol.SetParameters(new[] { Parameter.httprequesturi, Parameter.httprequestdata }, new object[] { url, request.Data });
			protocol.CheckTrigger(512);
		}

		public void Delete(SLProtocolExt protocol, string url, HttpDeleteRequest request)
		{
			protocol.Httprequesturi = url;
			protocol.CheckTrigger(513);
		}

	}
}
