using System.Text;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using SensorService.Configuration;

namespace SensorService.Logger
{
	public class BasicLogger : ILogger
	{

		private string rootPath;

		private string errorLogPath;
		private string messageLogPath;

		private object errorLock;
		private object msgLock;

		public BasicLogger(IHostingEnvironment hostEnv)
		{

			ServiceConfiguration config = ServiceConfiguration.read();

			this.rootPath = hostEnv.ContentRootPath;

			// special characters does not have to be escaped if @ is in front of the string
			this.errorLogPath = (this.rootPath + config.logErrorDest).Replace(@"//", @"/").Replace(@"\\", @"\");
			this.messageLogPath = (this.rootPath + config.logMsgDest).Replace(@"//", @"/").Replace(@"\\", @"\");

			this.errorLock = new object();
			this.msgLock = new object();

		}

		/*
			log format 

			log :  {

				tag: dev_error | dev_msg | prod_error | prod_msg,
				timestamp: 12/29/19 1:45:44 PM,
				content: "some string describin error or message ... "

			}

		*/

		public void logError(string error)
		{

			ServiceConfiguration config = ServiceConfiguration.read();

			JObject newLog = new JObject();
			newLog[config.logTagField] = config.logErrorTag;
			newLog[config.logTimeField] = DateTime.Now.ToString();
			newLog[config.logContentField] = error;

			lock (this.errorLock)
			{

				if (!File.Exists(this.errorLogPath))
				{
					File.Create(this.errorLogPath);
				}

				File.AppendAllText(this.errorLogPath, "\n" + newLog.ToString(), Encoding.UTF8);

			}

			if (config.consoleLogLevel.Contains(config.logErrorLevel))
			{
				Console.WriteLine(newLog.ToString());
			}

		}

		public void logMessage(string message)
		{


			ServiceConfiguration config = ServiceConfiguration.read();

			JObject newLog = new JObject();
			newLog[config.logTagField] = config.logMessageTag;
			newLog[config.logTimeField] = DateTime.Now.ToString();
			newLog[config.logContentField] = message;

			lock (this.msgLock)
			{

				if (!File.Exists(this.messageLogPath))
				{
					File.Create(this.messageLogPath);
				}

				File.AppendAllText(this.messageLogPath, "\n" + newLog.ToString());

			}

			if (config.consoleLogLevel.Contains(config.logMessageLevel))
			{
				Console.WriteLine(newLog.ToString());
			}

		}

	}
}