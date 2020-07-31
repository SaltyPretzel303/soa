
namespace CommunicationModel
{
	public class SensorRegistryRecord
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public int Port { get; set; }
		public int LastReadIndex { get; set; }

		public SensorRegistryRecord()
		{
		}

		public SensorRegistryRecord(string name, string address, int port, int lastReadIndex = 0)
		{
			this.Name = name;
			this.Address = address;
			this.Port = port;
			this.LastReadIndex = lastReadIndex;
		}
	}
}