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
using SensorService.Broker;
using CommunicationModel.BrokerModels;

namespace SensorService.Data
{
	public class SensorReader : IHostedService
	{

		private IMessageBroker broker;

		private System.Timers.Timer timer;
		private int lineCounter;
		private List<int> rowsCount;

		// injected services
		private ILogger logger;
		private IDataCacheManager DataCache;

		public SensorReader(ILogger logger, IDataCacheManager dataCache, IMessageBroker broker)
		{
			this.logger = logger;
			this.DataCache = dataCache;
			this.broker = broker;
		}

		private void ReadEvent(Object source, ElapsedEventArgs args)
		{

			ServiceConfiguration conf = ServiceConfiguration.Instance;
			// read one more line for evey sensor
			// read this.line_counter. row
			this.logger.logMessage(string.Join("",
							$"Read event sensorRange",
							$"({conf.sensorsRange.From}, ",
							$"{conf.sensorsRange.To})",
							$", rowCounter: {this.lineCounter}"));

			int logicIndex = 0;
			for (int realIndex = conf.sensorsRange.From; realIndex < conf.sensorsRange.To; realIndex++)
			{

				// in memory samples index
				logicIndex = realIndex - conf.sensorsRange.From;

				string samplePath = conf.dataPath +
								conf.samplePrefix +
								realIndex +
								conf.sampleExtension;

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

					this.DataCache.AddData(conf.sensorNamePrefix + realIndex,
										header,
										sensorRow);


					this.broker.PublishSensorEvent(new SensorReaderEvent(conf.sensorNamePrefix + realIndex,
																		this.lineCounter),
													conf.sensorReadEventFilter);

				}

			}

			this.lineCounter++;


		}

		#region IHostedService methods

		public Task StartAsync(CancellationToken cancellationToken)
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

			// read first row (identify comlumns)
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

			this.logger.logMessage("Reading interval: " + config.readInterval);

			this.timer = new System.Timers.Timer();
			this.timer.Elapsed += this.ReadEvent;
			this.timer.Interval = config.readInterval;
			this.timer.AutoReset = true;
			this.timer.Start();

			this.logger.logMessage("Reader is running ...  ");

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{

			Console.WriteLine("Sensor reader is going down ... ");
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