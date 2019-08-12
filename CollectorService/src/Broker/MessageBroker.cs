
using CollectorService.Broker.Events;
using RabbitMQ.Client;

namespace CollectorService.Broker
{
	public abstract class MessageBroker
	{

		private static MessageBroker instance;
		public static MessageBroker Instance
		{
			get
			{
				if (MessageBroker.instance == null)
				{
					MessageBroker.instance = new RabbitMqBroker();
				}

				return MessageBroker.instance;
			}
			private set { MessageBroker.instance = value; }
		}

		protected MessageBroker()
		{

		}

		public abstract IModel getChannel();

		public abstract void publishEvent(CollectorEvent eventToPublish);

	}
}