using System;
using System.Text;
using System.Threading.Tasks;
using CommunicationModel.BrokerModels;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SensorService.Configuration;

namespace SensorService.Broker
{
	public class RabbitMqBroker : IMessageBroker
	{

		private ServiceConfiguration config;

		public RabbitMqBroker()
		{
			this.config = ServiceConfiguration.Instance;
		}

		public async Task<bool> PublishLog(ServiceLog log)
		{
			return await Publish(log,
					config.serviceLogTopic,
					config.sensorLogFilter);
		}

		public async Task<bool> PublishSensorEvent(
			SensorReaderEvent sensorEvent,
			string filter)
		{
			return await this.Publish(
					sensorEvent,
					config.sensorReaderEventTopic,
					filter);
		}

		public async Task<bool> PublishLifetimeEvent(ServiceLifetimeEvent newEvent)
		{
			return await Publish(newEvent,
					config.serviceLifetimeTopic,
					config.sensorLifetimeFilter);
		}

		private async Task<bool> Publish(ServiceEvent serviceEvent, string topic, string filter)
		{

			IConnection connection = null;
			IModel channel = null;

			try
			{
				connection = await createConnection();
				channel = connection.CreateModel();

				string txtContent = JsonConvert.SerializeObject(serviceEvent);

				byte[] content = Encoding.UTF8.GetBytes(txtContent);

				channel.ExchangeDeclare(topic,
										"topic",
										true,
										true,
										null);

				channel.BasicPublish(topic,
									filter,
									false,
									null,
									content);


				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Failed to establish connection with message broker: address: {config.brokerAddress}:{config.brokerPort}, reason: {e.Message}");
				return false;
			}
			finally
			{

				if (channel != null && channel.IsOpen)
				{
					channel.Close();
				}

				if (connection != null && connection.IsOpen)
				{
					connection.Close();
				}
			}

		}

		private Task<IConnection> createConnection()
		{
			return Task.Run(() =>
			{
				var factory = new ConnectionFactory()
				{
					HostName = config.brokerAddress,
					Port = config.brokerPort
				};

				return factory.CreateConnection();
			});
		}

	}
}