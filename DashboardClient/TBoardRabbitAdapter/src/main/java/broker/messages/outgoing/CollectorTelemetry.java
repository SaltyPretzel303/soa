package broker.messages.outgoing;

import java.util.Date;

import broker.messages.incoming.CollectorAccessEvent;
import broker.messages.incoming.CollectorPullEvent;

public class CollectorTelemetry {

	public String type;

	// service event
	public String sourceId;
	public Date time;
	public String message;

	// pull event
	public String sensorAddress;
	public boolean pullSuccess;
	public int returnedCount;

	// access event
	public String method;
	public String requestPath;
	public String query;

	public String sourceAddr;
	public int sourcePort;

	public Date requestReceivedTime;
	public Date responseSendTime;

	public int statusCode;
	public String responseType;
	public long responseLength;

	public CollectorTelemetry(String sourceId, Date time, String message) {
		this.sourceId = sourceId;
		this.time = time;
		this.message = message;
	}

	public CollectorTelemetry(CollectorPullEvent pullEvent) {
		this(pullEvent.sourceId,
				pullEvent.time,
				pullEvent.customMessage);

		this.type = CollectorTelemetryType.Pull.toString();

		this.sensorAddress = pullEvent.sensorAddress;
		this.pullSuccess = pullEvent.success;
		this.returnedCount = pullEvent.returnedCount;
	}

	public CollectorTelemetry(CollectorAccessEvent accessEvent) {
		this(accessEvent.sourceId,
				accessEvent.time,
				accessEvent.customMessage);

		this.type = CollectorTelemetryType.Access.toString();

		this.method = accessEvent.method;
		this.requestPath = accessEvent.requestPath;
		this.query = accessEvent.query;
		this.sourceAddr = accessEvent.sourceAddr;
		this.sourcePort = accessEvent.sourcePort;
		this.requestReceivedTime = accessEvent.requestReceivedTime;
		this.responseSendTime = accessEvent.responseSendTime;
		this.statusCode = accessEvent.statusCode;
		this.responseType = accessEvent.responseType;
		this.responseLength = accessEvent.responseLength;
	}

}
