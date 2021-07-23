using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunicationModel.BrokerModels;
using MediatR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SensorRegistry.Configuration;
using SensorRegistry.Logger;
using SensorRegistry.MediatorRequests;

namespace SensorRegistry.Broker
{
	public class BrokerEventsReceiver : BackgroundService, IReloadable
	{
		private ILogger logger;

		private IConnection connection;
		private IModel channel;

		private IMediator mediator;

		private CancellationToken masterToken;

		private Task connectionRetryTask;
		private CancellationTokenSource connRetryTokenSrc;

		public BrokerEventsReceiver(
			ILogger logger,
			IMediator mediator)
		{
			this.logger = logger;
			this.mediator = mediator;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			masterToken = stoppingToken;
			masterToken.Register(() =>
			{
				// cancel connection retry if application shutdown is requested
				if (connRetryTokenSrc != null)
				{
					connRetryTokenSrc.Cancel();
				}
			});

			ServiceConfiguration.Instance.ListenForChange((IReloadable)this);

			var config = ServiceConfiguration.Instance;

			connRetryTokenSrc = new CancellationTokenSource();

			connectionRetryTask = establishConnection(connRetryTokenSrc.Token);
			await connectionRetryTask;

			connRetryTokenSrc = null;
			connectionRetryTask = null;

			if (masterToken.IsCancellationRequested)
			{
				if (connection != null &&
				connection.IsOpen)
				{
					connection.Close();
				}

				return;
			}

			Console.WriteLine("Setting up broker consumers ... ");

			this.setupConfigConsumer();
			this.setupSensorLifetimeConsumer();
			this.setupSensorReaderConsumer();
		}

		private Task establishConnection(CancellationToken token)
		{
			return Task.Run(async () =>
			{
				ServiceConfiguration config = ServiceConfiguration.Instance;

				bool connectionReady = false;
				while (!connectionReady &&
					!token.IsCancellationRequested)
				{

					connectionReady = ConfigureConnection();

					if (!connectionReady)
					{
						try
						{
							await Task.Delay(config.connectionRetryDelay, token);
						}
						catch (TaskCanceledException)
						{
							break;
						}
					}

				}

				if (token.IsCancellationRequested)
				{
					if (connection != null &&
					connection.IsOpen)
					{
						connection.Close();
					}
				}
				else
				{
					Console.WriteLine("Broker connection established ... ");
				}

			});
		}

		private bool ConfigureConnection()
		{
			var config = ServiceConfiguration.Instance;

			var connectionFactory = new ConnectionFactory
			{
				HostName = config.brokerAddress,
				Port = config.brokerPort
				// DispatchConsumersAsync = true
			};
			try
			{
				connection = connectionFactory.CreateConnection();
				channel = connection.CreateModel();
			}
			catch (Exception e)
			{
				logger.LogError($"Failed to create connection with broker: {e.Message}");
				return false;
			}

			if (this.connection != null &&
				this.connection.IsOpen &&
				this.channel != null &&
				this.channel.IsOpen)
			{

				this.channel.ExchangeDeclare(config.configUpdateTopic,
										"topic",
										true,
										true,
										null);

				this.channel.ExchangeDeclare(config.sensorEventTopic,
										"topic",
										true,
										true,
										null);

				return true;
			}

			return false;
		}

		private void setupConfigConsumer()
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			string configQueue = this.channel.QueueDeclare().QueueName;
			channel.QueueBind(configQueue,
				config.configUpdateTopic,
				config.configFilter,
				null);

			var configConsumer = new EventingBasicConsumer(this.channel);
			configConsumer.Received += (srcChannel, eventArg) =>
				{
					string txtContent = Encoding
						.UTF8
						.GetString(eventArg.Body.ToArray());
					JObject jsonContent = JObject.Parse(txtContent);

					this.mediator.Send(new ConfigUpdateRequest(jsonContent));
					// this.newConfigHandler.HandleNewConfig(jsonContent);
				};

			this.channel.BasicConsume(configQueue,
				true,
				configConsumer);

		}

		private void setupSensorLifetimeConsumer()
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			string rcvQueue = this.channel.QueueDeclare().QueueName;

			EventingBasicConsumer eventConsumer = new EventingBasicConsumer(this.channel);
			eventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding
					.UTF8
					.GetString(eventArg.Body.ToArray());

				var sensorEvent = JsonConvert
					.DeserializeObject<SensorLifetimeEvent>(txtContent);

				Console.WriteLine($"event => Lifetime: {sensorEvent.lifeStage.ToString()} "
					+ $"name: {sensorEvent.SensorName} "
					+ $"readIndex: {sensorEvent.LastReadIndex}");

				mediator.Send(new SensorLifetimeRequest(sensorEvent));
			};

			this.channel.QueueBind(rcvQueue,
							config.serviceLifetimeTopic,
							config.sensorLifetimeFilter,
							null);

			this.channel.BasicConsume(rcvQueue,
									true,
									eventConsumer);

		}

		private void setupSensorReaderConsumer()
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			string sensorReadEventQueue = this.channel.QueueDeclare().QueueName;

			var sensorEventConsumer = new EventingBasicConsumer(this.channel);
			sensorEventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding
					.UTF8
					.GetString(eventArg.Body.ToArray());
				var sensorEvent = JsonConvert
					.DeserializeObject<SensorReaderEvent>(txtContent);

				// Console.WriteLine($"event => Reader: {sensorEvent.SensorName} "
				// 	+ $"readIndex: {sensorEvent.LastReadIndex}");

				mediator.Send(new SensorUpdateRequest(sensorEvent));
			};

			this.channel.QueueBind(sensorReadEventQueue,
				config.sensorEventTopic,
				config.sensorReadEventFilter,
				null);

			this.channel.BasicConsume(sensorReadEventQueue,
				true,
				sensorEventConsumer);

		}

		public override Task StopAsync(CancellationToken cancellationToken)
		{

			if (this.channel != null && this.channel.IsOpen)
			{
				this.channel.Close();
			}

			if (this.connection != null && this.connection.IsOpen)
			{
				this.connection.Close();
			}

			return Task.CompletedTask;
		}

		public async void reload(ServiceConfiguration newConfiguration)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			if (this.connectionRetryTask != null)
			{
				if (this.connRetryTokenSrc != null)
				{
					this.connRetryTokenSrc.Cancel();
					await this.connectionRetryTask;

					this.connectionRetryTask = null;
					this.connRetryTokenSrc = null;
				}
			}

			if (this.channel != null &&
			this.channel.IsOpen)
			{
				this.channel.Close();
			}

			if (this.connection != null &&
			this.connection.IsOpen)
			{
				this.connection.Close();
			}

			this.connRetryTokenSrc = new CancellationTokenSource();
			this.connectionRetryTask =
				this.establishConnection(this.connRetryTokenSrc.Token);

			await this.connectionRetryTask;

			this.connRetryTokenSrc = null;
			this.connectionRetryTask = null;

			if (this.masterToken.IsCancellationRequested)
			{
				if (this.connection != null &&
				this.connection.IsOpen)
				{
					this.connection.Close();
				}

				return;
			}

			if (this.connection == null ||
			!this.connection.IsOpen)
			{
				return;
			}

			this.setupConfigConsumer();
			this.setupSensorReaderConsumer();

			Console.WriteLine("Broker event receiver reloaded ... ");
		}
	}
}