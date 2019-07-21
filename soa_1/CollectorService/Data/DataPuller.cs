using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Timers;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace CRUDService.Data
{
	public class DataPuller
	{

		static private String DATA_READER_ADDRESS = "http://localhost:5001";
		static private String HEADER_URL = "/data/reader/header";
		static private String DATA_RANGE_URL = "/data/reader/range";

		// database for storing pulled data
		public DatabaseService databse { get; private set; }
		// index of next (maybe current ) row that should be read
		public int read_index { get; private set; }

		// run specific code every read_interval miliseconds
		private Timer timer;
		public int read_interval { get; private set; }

		private HttpClient client;

		// constructors

		public DataPuller(DatabaseService databse, int read_interval)
		{
			this.databse = databse;
			this.read_interval = read_interval;

			// start reading from the first for (index 0)
			this.read_index = 0;

			// initialize timer for passed interval and specific event handler
			this.timer = new Timer();
			this.timer.Elapsed += new ElapsedEventHandler(this.timerEvent);
			this.timer.Interval = this.read_interval;
			this.timer.Enabled = true;

			this.client = new HttpClient();

		}

		// methods

		private void timerEvent(Object source, ElapsedEventArgs arg)
		{

			Console.WriteLine("PULLING DATA >>> " + this.read_index);

			String constructed_url = DataPuller.DATA_READER_ADDRESS + DataPuller.DATA_RANGE_URL + "?index=" + this.read_index;
			Console.WriteLine(constructed_url);
			Uri uri = new Uri(constructed_url);

			String response_data = this.client.GetStringAsync(uri).Result;

			Console.WriteLine("response <<< ");

			JObject json_response = JObject.Parse(response_data);
			Console.WriteLine("DataParsed ...");
			int user_num = (int)json_response.GetValue("users_count");

			Console.WriteLine("USERS NUM = " + user_num);

			// externalize somehow
			String user_name_prefix = "user_";
			String current_user_name = "";

			for (int user_index = 0; user_index < user_num; user_index++)
			{

				// user_0, user_1 ...
				current_user_name = user_name_prefix + user_index;

				JArray json_user_rows = (JArray)json_response.GetValue(current_user_name);

				this.databse.pushToUser(current_user_name, json_user_rows);

			}

			this.read_index += (int)json_response.GetValue("rows_count");

		}

		// not used ...
		// TODO remove
		private String requestHeader()
		{

			String response_data = "";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DataPuller.DATA_READER_ADDRESS + DataPuller.HEADER_URL);
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