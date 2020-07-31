using System.Text;
using System;
using CollectorService.Configuration;
using RabbitMQ.Client;
using CommunicationModel.BrokerModels;
using Newtonsoft.Json;

namespace CollectorService.Broker
{

	public class RabbitMqBroker : IMessageBroker
	{
		private ServiceConfiguration config;

		public RabbitMqBroker()
		{
			this.config = ServiceConfiguration.Instance;
		}

		public void PublishCollectorAccessEvent(CollectorAccessEvent newEvent)
		{
			this.PublishEvent(newEvent,
							this.config.collectorTopic,
							this.config.accessEventFilter);
		}

		public void PublishCollectorPullEvent(CollectorPullEvent newEvent)
		{
			this.PublishEvent(newEvent,
							this.config.collectorTopic,
							this.config.pullEventFilter);
		}

		public void PublishLifetimeEvent(ServiceLifetimeEvent newEvent)
		{
			this.PublishEvent(newEvent,
							this.config.serviceLifetimeTopic,
							this.config.serviceTypeFilter);
		}

		public void PublishLog(ServiceLog newLog)
		{
			this.PublishEvent(newLog,
							this.config.serviceLogTopic,
							this.config.serviceTypeFilter);
		}

		private void PublishEvent(ServiceEvent newEvent, string topic, string filter)
		{

			ConnectionFactory factory = new ConnectionFactory()
			{
				HostName = this.config.brokerAddress,
				Port = this.config.brokerPort
			};

			using (IConnection connection = factory.CreateConnection())
			using (IModel channel = connection.CreateModel())
			{

				string txtEvent = JsonConvert.SerializeObject(newEvent);
				byte[] byteContent = Encoding.UTF8.GetBytes(txtEvent);

				channel.ExchangeDeclare(topic,
										"topic",
										true,
										true,
										null);

				channel.BasicPublish(topic,
									filter,
									false,
									null,
									byteContent);

			}

		}


	}

}