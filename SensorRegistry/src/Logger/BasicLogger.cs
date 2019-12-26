using System;
using Newtonsoft.Json.Linq;

namespace SensorRegistry.src.Logger
{
	public class BasicLogger : ILogger
	{
		public string LogError(string error)
		{

			string timestamp = DateTime.Now.ToString();



			return timestamp;

		}


		public string LogMessage(string message)
		{

			string timestamp = DateTime.Now.ToString();

			JObject messageObject=new JObject();

			messageObject["timestamp"]=timestamp;
			messageObject[]


			return timestamp;

		}
	}
}