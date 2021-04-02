using System.Linq;
using System;
using System.IO;
using System.Timers;
using System.Collections.Generic;
using SensorService.Configuration;
using SensorService.Logger;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using SensorService.MediatorRequests;
using CommunicationModel;
using Newtonsoft.Json.Linq;

namespace SensorService.Data
{
	public class SensorReader : IHostedService
	{

		private IMediator Mediator;

		private ILogger Logger;
		private IDataCacheManager DataCache;

		private System.Timers.Timer timer;
		private int lineCounter;
		private List<int> rowsCount;

		public SensorReader(ILogger logger,
					IDataCacheManager dataCache,
					IMediator mediator)
		{
			this.Logger = logger;
			this.DataCache = dataCache;
			this.Mediator = mediator;
		}

		private void ReadEvent(Object source, ElapsedEventArgs args)
		{

			ServiceConfiguration conf = ServiceConfiguration.Instance;
			// read one more line for every sensor
			// read this.line_counter. row
			this.Logger.logMessage(string.Join("",
							$"Read event sensorRange",
							$"({conf.sensorsRange.From}, ",
							$"{conf.sensorsRange.To})",
							$", rowCounter: {this.lineCounter}"));

			int logicIndex = 0;
			for (int realIndex = conf.sensorsRange.From;
					realIndex < conf.sensorsRange.To;
					realIndex++)
			{

				// in memory samples index
				logicIndex = realIndex - conf.sensorsRange.From;

				string samplePath = conf.dataPath
								+ conf.samplePrefix + realIndex
								+ conf.sampleExtension;

				List<string> header = File.ReadLines(samplePath).
												Take(1).
												First().
												Split(",").
												ToList();

				// index represents one user (user index)
				// if this record (this user) actually has this much lines
				if (this.rowsCount[logicIndex] > this.lineCounter)
				{

					string sensorRow = File.ReadLines(samplePath).
													Skip(this.lineCounter).
													Take(1).
													First();

					string sensorName = conf.sensorNamePrefix + realIndex;

					this.Mediator.Send(new StoreSensorDataRequest(sensorName,
												header,
												this.lineCounter,
												fromCsv(header, sensorRow)));

					// this.DataCache.AddData(conf.sensorNamePrefix + realIndex,
					// 					header,
					// 					sensorRow);


					// this.broker.PublishSensorEvent(new SensorReaderEvent(sensorName,
					// 										header,
					// 										this.lineCounter,
					// 										sensorRow,
					// 										conf.hostIP,
					// 										conf.listeningPort),
					// 					conf.sensorReadEventFilter);

				}

			}

			this.lineCounter++;


		}

		private SensorValues fromCsv(List<string> header, string row)
		{
			string[] values = row.Split(",");

			JObject jObj = new JObject();
			int index = 0;

			foreach (string field in header)
			{
				jObj[field] = values[index];
				index++;
			}

			return jObj.ToObject<SensorValues>();
		}

		#region IHostedService methods

		public Task StartAsync(CancellationToken cancellationToken)
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

			// read first row (identify columns)
			string samplePath = config.dataPath +
								config.samplePrefix +
								config.sensorsRange.From +
								config.sampleExtension;

			// initialize the number of available lines for every user
			this.rowsCount = new List<int>();

			for (int sensorNum = config.sensorsRange.From;
					sensorNum < config.sensorsRange.To;
					sensorNum++)
			{
				this.rowsCount.Add(File.ReadLines(config.dataPath +
													config.samplePrefix +
													sensorNum +
													config.sampleExtension).Count());
			}

			// ignore first line - header row
			this.lineCounter = 1;

			this.Logger.logMessage("Reading interval: " + config.readInterval);

			this.timer = new System.Timers.Timer();
			this.timer.Elapsed += this.ReadEvent;
			this.timer.Interval = config.readInterval;
			this.timer.AutoReset = true;
			this.timer.Start();

			this.Logger.logMessage("Reader is running ...  ");

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{

			if (this.timer != null)
			{
				this.timer.Stop();
			}

			Console.WriteLine("Sensor reader is down ... ");

			return Task.CompletedTask;
		}

		#endregion

	}

}