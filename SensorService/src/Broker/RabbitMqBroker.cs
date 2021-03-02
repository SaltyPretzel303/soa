using System;
using System.Text;
using CommunicationModel.BrokerModels;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SensorService.Configuration;

namespace SensorService.Broker
{
	public class RabbitMqBroker : IMessageBroker
	{

		public RabbitMqBroker()
		{
		}

		public void PublishLog(ServiceLog log)
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

			this.Publish(log,
					config.serviceLogTopic,
					config.sensorLogFilter);
		}

		public void PublishSensorEvent(SensorReaderEvent sensorEvent, string filter)
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

			this.Publish(sensorEvent,
					config.sensorReaderEventTopic,
					filter);
		}

		public void PublishLifetimeEvent(ServiceLifetimeEvent newEvent)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			this.Publish(newEvent,
					config.serviceLifetimeTopic,
					config.sensorLifetimeFilter);
		}

		private void Publish(ServiceEvent serviceEvent, string topic, string filter)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			ConnectionFactory factory = new ConnectionFactory()
			{
				HostName = config.brokerAddress,
				Port = config.brokerPort
			};

			IConnection connection = null;
			IModel channel = null;

			try
			{

				connection = factory.CreateConnection();
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

			}
			catch (Exception e)
			{
				Console.WriteLine($"Failed to establish connection with message broker: address: {config.brokerAddress}:{config.brokerPort}, reason: {e.Message}");
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

	}
}