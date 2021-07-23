package broker.handlers;

import java.util.UUID;

public class DeviceData {

	public UUID tBoardId;
	// for sensors this is gonna be their name (sensor_0)
	public String soaId;
	public String token;
	// public String deviceName;

	public DeviceData(UUID tBoardId,
			String soaId,
			String token
	// , String deviceName
	) {

		this.soaId = soaId;
		this.tBoardId = tBoardId;
		this.token = token;
		// this.deviceName = deviceName;
	}

}
