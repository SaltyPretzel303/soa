using System.Text;
using System;
using SensorRegistry.Configuration;
using RabbitMQ.Client;
using CommunicationModel.BrokerModels;
using Newtonsoft.Json;

namespace SensorRegistry.Broker
{
	public class RabbitMqBroker : IMessageBroker
	{
		public void publishRegistryEvent(SensorRegistryEvent newEvent, string filter)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;
			this.PublishEvent(newEvent,
							config.sensorRegistryTopic,
							filter);
		}

		public void publishLifetimeEvent(ServiceLifetimeEvent ltEvent)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;
			this.PublishEvent(ltEvent,
							config.serviceLifetimeTopic,
							config.registryLifetimeFilter);
		}

		public void publishLog(ServiceLog log)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;
			this.PublishEvent(log,
							config.serviceLogTopic,
							config.registryLogFilter);
		}

		private void PublishEvent(ServiceEvent newEvent, string topicName, string filter)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			ConnectionFactory connFactory = new ConnectionFactory()
			{
				HostName = config.brokerAddress,
				Port = config.brokerPort
			};

			IConnection connection = null;
			IModel channel = null;

			try
			{
				string txtEvent = JsonConvert.SerializeObject(newEvent);
				byte[] content = Encoding.UTF8.GetBytes(txtEvent);

				connection = connFactory.CreateConnection();
				channel = connection.CreateModel();

				channel.ExchangeDeclare(topicName,
									"topic",
									true,
									true,
									null);

				channel.BasicPublish(topicName,
									filter,
									false,
									null,
									content);

			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to establish connection with broker: "
					+ $"address: {config.brokerAddress}:{config.brokerPort}, "
					+ $"reason: {e.Message}");
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