namespace QAction_1.Http
{
	using Skyline.DataMiner.Scripting;

	public interface IHttpActions
	{
		void Get(SLProtocolExt protocol, string url, HttpGetRequest request);

		void Post(SLProtocolExt protocol, string url, HttpPostRequest request);

		void Put(SLProtocolExt protocol, string url, HttpPutRequest request);

		void Delete(SLProtocolExt protocol, string url, HttpDeleteRequest request);

	}
}
