package broker.messages.incoming;

public class SensorRegistryRecord {

	public String Name;
	public String Address;
	public int Port;
	public long AvailableRecords;

	public SensorRegistryRecord(
			String name,
			String address,
			int port,
			int availableRecords) {

		this.Name = name;
		this.Address = address;
		this.Port = port;
		this.AvailableRecords = availableRecords;
	}
}
