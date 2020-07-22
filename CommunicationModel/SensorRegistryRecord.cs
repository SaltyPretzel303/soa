
namespace CommunicationModel
{
	public class SensorRegistryRecord
	{
		public string name { get; set; }
		public string address { get; set; }
		public int port { get; set; }

		public SensorRegistryRecord()
		{

		}

		public SensorRegistryRecord(string name, string address, int port)
		{
			this.name = name;
			this.address = address;
			this.port = port;
		}
	}
}