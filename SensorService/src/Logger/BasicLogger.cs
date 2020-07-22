using System.Text;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using SensorService.Configuration;
using System.Text.Json;

namespace SensorService.Logger
{
	public class BasicLogger : ILogger
	{

		private string rootPath;

		private string errorLogPath;
		private string messageLogPath;

		private object errorLock;
		private object msgLock;

		public BasicLogger(IWebHostEnvironment hostEnv)
		{

			ServiceConfiguration config = ServiceConfiguration.Read();

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

			ServiceConfiguration config = ServiceConfiguration.Read();

			ServiceLog newLog = new ServiceLog(config.logErrorTag,
											DateTime.Now.ToString(),
											error);

			string serializedLog = JsonSerializer.Serialize(newLog);

			lock (this.errorLock)
			{

				if (!File.Exists(this.errorLogPath))
				{
					File.Create(this.errorLogPath).Close();
				}

				File.AppendAllText(this.errorLogPath, "\n" + serializedLog, Encoding.UTF8);

			}

			if (config.consoleLogLevel.Contains(config.logErrorLevel))
			{
				Console.WriteLine(serializedLog);
			}

		}

		public void logMessage(string message)
		{


			ServiceConfiguration config = ServiceConfiguration.Read();

			ServiceLog newLog = new ServiceLog(config.logMessageTag,
											DateTime.Now.ToString(),
											message);

			string serializedLog = JsonSerializer.Serialize(newLog);

			lock (this.msgLock)
			{

				if (!File.Exists(this.messageLogPath))
				{
					File.Create(this.messageLogPath).Close();
				}

				File.AppendAllText(this.messageLogPath, "\n" + serializedLog);

			}

			if (config.consoleLogLevel.Contains(config.logMessageLevel))
			{
				Console.WriteLine(serializedLog);
			}

		}

	}

}