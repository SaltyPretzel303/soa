using System;

namespace CommunicationModel.RestModels
{
	public class ConfigRecord
	{
		public string txtConfig { get; set; }
		public DateTime backupDate { get; set; }

		public ConfigRecord(string txtConfig, DateTime backupDate)
		{
			this.txtConfig = txtConfig;
			this.backupDate = backupDate;
		}
	}
}