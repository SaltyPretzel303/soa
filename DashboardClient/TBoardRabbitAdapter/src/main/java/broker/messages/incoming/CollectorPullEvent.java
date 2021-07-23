package broker.messages.incoming;

public class CollectorPullEvent extends ServiceEvent {

	public String sensorAddress;
	public boolean success;
	public int returnedCount;

	public CollectorPullEvent(
			String sourceId,
			ServiceType type,
			String customMessage,
			String sensorAddress,
			boolean success,
			int returnedCount) {
		super(sourceId, type, customMessage);

		this.sensorAddress = sensorAddress;
		this.success = success;
		this.returnedCount = returnedCount;

	}

}
