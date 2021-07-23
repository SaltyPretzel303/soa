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
using CommunicationModel;
using Newtonsoft.Json.Linq;
using SensorService.MediatorRequests;

namespace SensorService.Data
{

	public class SensorReader : IHostedService
	{

		private IMediator mediator;

		private ILogger Logger;

		private System.Timers.Timer timer;

		private Dictionary<string, SensorDataInfo> sensors;

		public SensorReader(ILogger logger, IMediator mediator)
		{
			this.Logger = logger;
			this.mediator = mediator;

			this.sensors = new Dictionary<string, SensorDataInfo>();
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			await PreprocessData(cancellationToken);

			this.Logger.logMessage("Reading interval: " + config.readInterval);

			this.timer = new System.Timers.Timer();
			this.timer.Elapsed += this.ReadEvent;
			this.timer.Interval = config.readInterval;
			this.timer.AutoReset = false;

			this.timer.Start();

			this.Logger.logMessage("Reader is running ...  ");
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

		private async Task PreprocessData(CancellationToken token)
		{
			var config = ServiceConfiguration.Instance;

			for (int index = config.sensorsRange.From;
				index < config.sensorsRange.To;
				index++)
			{

				string sensorName = config.sensorNamePrefix + index;
				string sensorFileName = config.dataPath
					+ config.samplePrefix
					+ index
					+ config.sampleExtension;

				var info = new SensorDataInfo(sensorName, sensorFileName);

				sensors.Add(sensorName, info);

				// find start and end position of every line (excluding header line)
				using (var reader = File.OpenText(info.FileName))
				{
					string headerLine = reader.ReadLine();
					info.CsvHeader = headerLine;

					int lastIndex = headerLine.Length + 1;

					string line = "";
					while ((line = await reader.ReadLineAsync()) != null)
					{
						var borders = new LineBorders()
						{
							StartPos = lastIndex,
							EndPos = lastIndex + line.Length
						};

						info.Lines.Add(borders);

						lastIndex += line.Length + 1;
					}

				}

			}

		}

		private async void ReadEvent(Object source, ElapsedEventArgs args)
		{
			timer.Stop();

			var toRemove = new List<string>();
			foreach (var sensor in sensors.Values.ToList())
			{
				if (sensor.ReadIndex >= sensor.Lines.Count)
				{
					Console.WriteLine($"No more data to read for this sensor\n"
						+ $"\tId: {sensor.SensorName} Lines: {sensor.ReadIndex}");

					toRemove.Add(sensor.SensorName);

					continue;
				}

				var lineBorders = sensor.Lines[sensor.ReadIndex];
				int lineLen = lineBorders.EndPos - lineBorders.StartPos;

				byte[] buff = new byte[lineLen];

				using (var reader = File.OpenRead(sensor.FileName))
				{
					reader.Position = lineBorders.StartPos;
					await reader.ReadAsync(buff, 0, lineLen);
				}

				string line = System.Text.Encoding.ASCII.GetString(buff);

				await mediator.Send(
					new StoreSensorDataRequest(
						sensor.SensorName,
						sensor.CsvHeader,
						sensor.ReadIndex,
						line)
					);

				sensor.ReadIndex++;
			}

			if (toRemove.Count > 0)
			{
				foreach (var name in toRemove)
				{
					this.sensors.Remove(name);
				}
			}

			timer.Start();

		}

	}

}