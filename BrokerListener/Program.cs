using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System;

namespace BrokerListener
{
	class Program
	{
		static void Main(string[] args)
		{

			List<string> topics = new List<string>{
				"service_log",
				"service_lifetime",
				"sensor_registry",
				"test_topic",
				"sensor_reader",
			};

			string filter = "#";

			ConnectionFactory connFactory = new ConnectionFactory()
			{
				HostName = "localhost",
				Port = 5672
			};

			using (IConnection connection = connFactory.CreateConnection())
			using (IModel channel = connection.CreateModel())
			{

				foreach (string topic in topics)
				{


					Console.WriteLine($"Creating {topic} topic ... ");

					channel.ExchangeDeclare(topic,
										"topic",
										true,
										true,
										null);

					string queue = channel.QueueDeclare().QueueName;

					EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
					consumer.Received += (srcChannel, eventArg) =>
					{

						string payload = System.Text.Encoding.UTF8.GetString(eventArg.Body.ToArray());
						JObject jPayload = JObject.Parse(payload);

						Console.WriteLine($"On topic ({topic}) received:\n {jPayload.ToString()}");
					};
					channel.QueueBind(queue,
									topic,
									filter,
									null);

					channel.BasicConsume(queue,
										false,
										consumer);

				}

				Console.ReadLine();
			}


		}

	}
}
