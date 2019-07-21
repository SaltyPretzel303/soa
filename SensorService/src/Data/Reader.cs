using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Linq;
using System;
using System.IO;
using System.Timers;
using System.Collections.Generic;

namespace DataCollector.Data
{

	public class Reader
	{

		private Timer timer;

		public String path { get; private set; }
		public String prefix { get; private set; }
		public String extension { get; private set; }
		public int line_counter { get; private set; }

		public int read_interval { get; private set; }

		public int users_count { get; private set; }
		public List<int> lines_count;

		private List<String> columns;
		
		// every list inside this list represents rows for user with same index
		private List<List<String>> data;

		public Reader(String path, String prefix, String extension, int users_count, int read_interval)
		{

			this.path = path;
			this.prefix = prefix;
			this.extension = extension;
			this.users_count = users_count;
			this.read_interval = read_interval;

			// read first row (identify comlumns)
			this.columns = new List<String>(File.ReadLines(this.path + prefix + 0 + extension).Take(1).First().Split(","));

			// initialize the number of available lines for every user
			this.lines_count = new List<int>();
			for (int index = 0; index < this.users_count; index++)
			{
				this.lines_count.Add(File.ReadLines(this.path + this.prefix + index + this.extension).Count());

			}

			// initialize list for every user
			this.data = new List<List<String>>();
			for (int i = 0; i < this.users_count; i++)
			{
				this.data.Add(new List<String>());
			}

			// ignore first line (row with columns names)
			this.line_counter = 1;

			this.timer = new Timer();
			this.timer.Elapsed += new ElapsedEventHandler(this.ReadEvent);
			this.timer.Interval = this.read_interval;
			this.timer.Enabled = true;

		}

		private void ReadEvent(Object source, ElapsedEventArgs args)
		{

			// read one more line for evey user
			// read this.line_counter row
			Console.WriteLine("Read event ...");
			for (int index = 0; index < this.users_count; index++)
			{
				// index represents one user (user index)
				// if this record (this user) actually has this much lines
				if (this.lines_count[index] > this.line_counter)
				{

					// read line
					// skip previous read lines (this.line_counter)
					this.data[index].Add(File.ReadLines(this.path + this.prefix + index + this.extension).Skip(this.line_counter).Take(1).First());

				}


			}

			this.line_counter++;

		}

		public List<List<String>> getDataFrom(int index)
		{

			if (index < this.line_counter && index >= 0)
			{
				List<List<String>> ret_list = new List<List<String>>();

				foreach (List<String> user in this.data)
				{
					// get lines from single user
					ret_list.Add(user.GetRange(index, user.Count - index));

				}

				return ret_list;

			}

			return null;
		}

		public List<String> getHeader()
		{
			return this.columns;
		}

	}
}