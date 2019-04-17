using System;
using System.Timers;

namespace CRUDService.Data
{
	public class DataManager
	{

		public DatabaseService databse { get; private set; }
		public int read_index { get; private set; }

		private Timer timer;
		public int read_interval { get; private set; }

		public DataManager(DatabaseService databse, int read_interval)
		{
			this.databse = databse;
			this.read_interval = read_interval;

			this.read_index = 0;

			this.timer = new Timer();
			this.timer.Elapsed += new ElapsedEventHandler(this.timerEvent);
			this.timer.Interval = this.read_interval;
			this.timer.Enabled = true;

		}

		private void timerEvent(Object soure, ElapsedEventArgs args)
		{

			// use rest api from another service
			// parse data
			// increase read_index
			// save data

		}

		private String collectData()
		{

			return "";
		}

		private void saveData(String data)
		{

		}

	}
}