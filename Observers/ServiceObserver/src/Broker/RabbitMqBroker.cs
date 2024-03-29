using System;
using System.Text;
using CommunicationModel.BrokerModels;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ServiceObserver.Configuration;

namespace ServiceObserver.Broker
{
	public class RabbitMqBroker : IMessageBroker
	{
		private ConfigFields config;

		public RabbitMqBroker()
		{
			this.config = ServiceConfiguration.Instance;
		}

		public void PublishLifetimeEvent(ServiceLifetimeEvent ltEvent)
		{
			this.PublishEvent(ltEvent,
							config.serviceLifetimeTopic,
							config.serviceTypeFilter);
		}

		public void PublishLog(ServiceLog log)
		{
			this.PublishEvent(log,
							config.serviceLogTopic,
							config.serviceTypeFilter);
		}

		public void PublishObserverReport(
			ServiceEvent newReport,
			string resultTypeFilter)
		// passed filter represents only observing result type (unstable service)
		{
			string completeFilter =
				config.observingResultsFilter
				+ "."
				+ resultTypeFilter;

			this.PublishEvent(
				newReport,
				config.observingResultsTopic,
				completeFilter);
		}

		private void PublishEvent(ServiceEvent newEvent,
							string topicName,
							string filter)
		{
			ConnectionFactory connFactory = new ConnectionFactory()
			{
				HostName = config.brokerAddress,
				Port = config.brokerPort
			};

			IConnection connection = null;
			IModel channel = null;

			try
			{

				connection = connFactory.CreateConnection();
				channel = connection.CreateModel();

				string txtEvent = JsonConvert.SerializeObject(newEvent);
				byte[] content = Encoding.UTF8.GetBytes(txtEvent);

				// Console.WriteLine("Publishing: " + txtEvent);

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
				Console.WriteLine($"Failed to establish connection with message broker: "
					+ $"address: {config.brokerAddress}:{config.brokerPort}, "
					+ $"reason: {e.Message} ");

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