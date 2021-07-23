package broker.messages.incoming;

import java.util.List;

public class SensorReaderEvent extends ServiceEvent {

	public String SensorName;
	public int LastReadIndex;

	public String IpAddress;
	public int ListeningPort;

	public List<String> DataHeader;
	public SensorValues NewData;

	public SensorReaderEvent(
			String sourceId,
			ServiceType type,
			String message,
			String SensorName,
			int LastReadIndex,
			String IpAddress,
			int ListeningPort,
			List<String> DataHeader,
			SensorValues NewData) {
		super(sourceId, type, message);

		this.SensorName = SensorName;
		this.LastReadIndex = LastReadIndex;
		this.IpAddress = IpAddress;
		this.ListeningPort = ListeningPort;
		this.DataHeader = DataHeader;
		this.NewData = NewData;
	}

}
