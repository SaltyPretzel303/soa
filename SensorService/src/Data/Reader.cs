using System.Linq;
using System;
using System.IO;
using System.Timers;
using System.Collections.Generic;
using SensorService.Configuration;

namespace SensorService.Data
{

	public class Reader
	{

		private Timer timer;

		public FromTo samplesRange { get; private set; }
		public String path { get; private set; }
		public String prefix { get; private set; }
		public String extension { get; private set; }
		public int lineCounter { get; private set; }

		public int readInterval { get; private set; }

		public List<int> rows_count;

		private List<String> columns;

		// every list inside this list represents rows for user with same index
		// single string represents one row - collections of values for every column
		private List<List<String>> data;

		// constructors

		public Reader(String path, String prefix, String extension, FromTo samples_range, int read_interval)
		{

			this.path = path;
			this.prefix = prefix;
			this.extension = extension;
			this.samplesRange = samples_range;
			this.readInterval = read_interval;

			// read first row (identify comlumns)
			this.columns = new List<String>(File.ReadLines(this.path + this.prefix + this.samplesRange.From + this.extension).Take(1).First().Split(","));

			// initialize the number of available lines for every user
			this.rows_count = new List<int>();
			for (int sample_num = this.samplesRange.From; sample_num < this.samplesRange.To; sample_num++)
			{
				this.rows_count.Add(File.ReadLines(this.path + this.prefix + sample_num + this.extension).Count());
			}

			// initialize list for every user
			this.data = new List<List<String>>();
			for (int i = samplesRange.From; i < samplesRange.To; i++)
			{
				this.data.Add(new List<String>());
			}

			// ignore first line (row with names of the columns)
			this.lineCounter = 1;

			this.timer = new Timer();
			this.timer.Elapsed += new ElapsedEventHandler(this.ReadEvent);
			this.timer.Interval = this.readInterval;
			this.timer.Enabled = true;

		}

		// private methods

		private void ReadEvent(Object source, ElapsedEventArgs args)
		{

			// read one more line for evey user
			// read this.line_counter row
			Console.WriteLine("Read event ...");
			int logical_index = 0;
			for (int real_index = this.samplesRange.From; real_index < this.samplesRange.To; real_index++)
			{
				// in memory samples index
				logical_index = real_index - this.samplesRange.From;

				// index represents one user (user index)
				// if this record (this user) actually has this much lines
				if (this.rows_count[logical_index] > this.lineCounter)
				{

					// read line
					// skip previous read lines (this.line_counter)
					this.data[logical_index].Add(File.ReadLines(this.path + this.prefix + real_index + this.extension).Skip(this.lineCounter).Take(1).First());

				}


			}

			this.lineCounter++;

		}

		// public methods

		public List<List<String>> getDataFrom(int index)
		{

			if (index < this.lineCounter && index >= 0)
			{

				List<List<String>> ret_list = new List<List<String>>();

				foreach (List<String> user_rows in this.data)
				{
					// get rows from single user
					if (user_rows.Count >= index)
					{
						ret_list.Add(user_rows.GetRange(index, user_rows.Count - index)); // second argument is count 
					}

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