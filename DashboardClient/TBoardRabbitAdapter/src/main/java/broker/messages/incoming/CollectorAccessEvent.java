package broker.messages.incoming;

import java.util.Date;

public class CollectorAccessEvent extends ServiceEvent {

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

	public CollectorAccessEvent(String sourceId,
			ServiceType type,
			String customMessage,
			String method,
			String requestPath,
			String query,
			String sourceAddr,
			int sourcePort,
			Date requestReceivedTime,
			Date responseSendTime,
			int statusCode,
			String responseType,
			long responseLength) {

		super(sourceId, type, customMessage);

		this.method = method;
		this.requestPath = requestPath;
		this.query = query;
		this.sourceAddr = sourceAddr;
		this.sourcePort = sourcePort;
		this.requestReceivedTime = requestReceivedTime;
		this.responseSendTime = responseSendTime;
		this.statusCode = statusCode;
		this.responseType = responseType;
		this.responseLength = responseLength;
	}

}
