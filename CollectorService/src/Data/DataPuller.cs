using System.Net.Sockets;
using System.IO;
using System.Net;
using System;
using System.Timers;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Collections.Generic;
using CollectorService.Broker;

namespace CollectorService.Data
{
	public class DataPuller
	{

		public IDatabaseService database { get; private set; }
		// index of the next row that should be read
		public int read_index { get; private set; }

		private Timer timer;
		public int read_interval { get; private set; }

		private HttpClient httpClient;

		private MessageBroker broker;

		private List<string> sensors_addr;

		private string dataRangeUrl;
		private string headerUrl;

		// constructors

		public DataPuller(IDatabaseService databse, MessageBroker broker, int read_interval, List<string> sensors, string data_range_url, string header_url)
		{

			this.database = databse;
			this.broker = broker;

			this.read_interval = read_interval;
			this.sensors_addr = sensors;
			this.dataRangeUrl = data_range_url;
			this.headerUrl = header_url;

			// start reading from the first for (index 0)
			this.read_index = 0;

			// initialize timer for passed interval and specific event handler
			this.timer = new Timer();
			this.timer.Elapsed += new ElapsedEventHandler(this.timerEvent);
			this.timer.Interval = this.read_interval;
			this.timer.Enabled = true;

			this.httpClient = new HttpClient();

		}

		// methods

		private void timerEvent(Object source, ElapsedEventArgs arg)
		{
			int max_row_count = -1;
			foreach (String single_sensor in this.sensors_addr)
			{

				string api_url = single_sensor + this.dataRangeUrl + "?index=" + this.read_index;
				Uri sensor_uri = new Uri(api_url);
				Console.WriteLine("Pulling from: " + api_url);

				string s_response = "";
				try
				{
					s_response = this.httpClient.GetStringAsync(sensor_uri).Result;
				}
				catch (AggregateException e)
				{
					Console.WriteLine("Aggregate exception: ");

					if (e.InnerException is HttpRequestException)
					{

						string message = ((HttpRequestException)e.InnerException).Message;

						Console.WriteLine($"Http req. exception, message: {message} ... sensor may be down.\nSensor addr. : {sensor_uri.ToString()}\n");

					}


				}
				catch (Exception e)
				{
					Console.WriteLine($"UNKNOWN EXCEPTION in gettin data: {e.ToString()}");
					continue;
				}
				JObject j_response = JObject.Parse(s_response);

				int row_count = int.Parse(j_response.GetValue("rows_count").ToString());
				if (row_count > max_row_count)
				{
					max_row_count = row_count;
				}
				int from_index = int.Parse(j_response.GetValue("from_sample").ToString());
				int to_index = int.Parse(j_response.GetValue("to_sample").ToString());
				string sensor_prefix = j_response.GetValue("sensor_name_prefix").ToString();

				string temp_sample_name = "";

				for (int sample_index = from_index; sample_index < to_index; sample_index++)
				{

					temp_sample_name = sensor_prefix + sample_index.ToString();

					JArray sample_values = (JArray)j_response.GetValue(temp_sample_name);

					this.database.pushToSensor(temp_sample_name, sample_values);

				}

				Console.WriteLine($"Got samples from: {from_index} to: {to_index}");


			}

			Console.WriteLine("Read index increase for: " + max_row_count);
			if (max_row_count > 0)
				this.read_index += max_row_count;
			Console.WriteLine("New read index: " + this.read_index);
		}

		// not used ...
		// TODO remove
		private String requestHeader()
		{

			String response_data = "";

			// all sensors have same header
			string single_sensor = this.sensors_addr[0];

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(single_sensor + this.headerUrl);
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				response_data = reader.ReadToEnd();
			}

			Console.WriteLine("DEBUG: \n");
			Console.WriteLine(response_data);

			return response_data;
		}

	}
}