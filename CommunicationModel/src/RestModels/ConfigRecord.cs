using System;

namespace CommunicationModel.RestModels
{
	public class ConfigRecord
	{
		public string serviceId { get; set; }
		public string txtConfig { get; set; }
		public DateTime backupDate { get; set; }

		public ConfigRecord(string serviceId, string txtConfig, DateTime backupDate)
		{
			this.serviceId = serviceId;
			this.txtConfig = txtConfig;
			this.backupDate = backupDate;
		}
	}
}