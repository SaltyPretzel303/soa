using System.Text;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using SensorService.Configuration;
using System.Text.Json;
using CommunicationModel.BrokerModels;

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

			ServiceConfiguration config = ServiceConfiguration.Instance;

			this.rootPath = hostEnv.ContentRootPath;

			// special characters does not have to be escaped if @ is in front of the string
			this.errorLogPath = (this.rootPath + config.logErrorDest).Replace(@"//", @"/").Replace(@"\\", @"\");
			this.messageLogPath = (this.rootPath + config.logMsgDest).Replace(@"//", @"/").Replace(@"\\", @"\");

			this.errorLock = new object();
			this.msgLock = new object();
		}

		public void logError(string error, bool online = true)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			ServiceLog newLog = new ServiceLog(error, LogLevel.Error);

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

		public void logMessage(string message, bool online = true)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			ServiceLog newLog = new ServiceLog(message, LogLevel.Message);

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