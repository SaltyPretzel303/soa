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
							"");
		}

		public void publishLifetimeEvent(ServiceLifetimeEvent ltEvent)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;
			this.PublishEvent(ltEvent,
							config.serviceLifetimeTopic,
							config.collectorLifetimeFilter);
		}

		public void publishLog(ServiceLog log)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;
			this.PublishEvent(log,
							config.serviceLogTopic,
							config.collectorLogFilter);
		}

		private void PublishEvent(ServiceEvent newEvent, string topicName, string filter)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			ConnectionFactory connFactory = new ConnectionFactory()
			{
				HostName = config.brokerAddress,
				Port = config.brokerPort
			};

			using (IConnection connection = connFactory.CreateConnection())
			using (IModel channel = connection.CreateModel())
			{

				channel.ExchangeDeclare(topicName, "topic", true, true, null);

				string txtEvent = JsonConvert.SerializeObject(newEvent);
				byte[] content = Encoding.UTF8.GetBytes(txtEvent);
				channel.BasicPublish(topicName,
									filter,
									false,
									null,
									content);

				Console.WriteLine("Publishing: " + txtEvent);
			}
		}

	}

}