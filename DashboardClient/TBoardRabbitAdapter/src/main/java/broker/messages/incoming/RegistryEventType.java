package broker.messages.incoming;

import com.google.gson.annotations.SerializedName;

public enum RegistryEventType {
	@SerializedName("0")
	NewSensor, @SerializedName("1")
	SensorRemoved, @SerializedName("2")
	SensorUpdated
}
