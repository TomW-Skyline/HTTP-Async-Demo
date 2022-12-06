using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using QAction_1.Http;

using Skyline.DataMiner.Scripting;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public class QAction
{
	private readonly HttpClient _httpClient = new HttpClient();

	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public void Run(SLProtocolExt protocol)
	{
		try
		{
			var trigger = protocol.GetTriggerParameter();

			switch (trigger)
			{
				case Parameter.trigger1min:
					Task.Run(() => GetData(protocol));
					break;
				case Parameter.httpaftergroup:
					ProcessResponse(protocol);
					break;
				default:
					throw new InvalidOperationException("Invalid trigger: " + trigger);
			}
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}

	private async Task GetData(SLProtocolExt protocol)
	{
		try
		{
			var urls = new[]
			{
				"https://www.skyline.be",
				"https://www.google.com",
				"https://www.microsoft.com",
			};

			var responses = new List<HttpResponse>();

			foreach (var url in urls)
			{
				var response = await _httpClient.Get(protocol, url);

				responses.Add(response);
			}

			UpdateParameters(protocol, responses);
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|GetData|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}

	private void ProcessResponse(SLProtocolExt protocol)
	{
		var values = (object[])protocol.GetParameters(new uint[]
		{
			Parameter.httpstatuscode_501,
			Parameter.httpresponsedata_520,
		});

		var resultCode = Convert.ToString(values[0]);
		var response = Convert.ToString(values[1]);

		_httpClient.RegisterResponse(protocol, resultCode, response);
	}

	private void UpdateParameters(SLProtocolExt protocol, ICollection<HttpResponse> responses)
	{
		foreach (var r in responses)
		{
			protocol.Log(r.Request.Url + ":" + r.ResultCode + Environment.NewLine + r.Response);
		}
	}
}
