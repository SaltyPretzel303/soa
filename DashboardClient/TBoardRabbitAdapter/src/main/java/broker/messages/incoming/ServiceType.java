package broker.messages.incoming;

import com.google.gson.annotations.SerializedName;

public enum ServiceType {
	@SerializedName("0")
	SensorReader, @SerializedName("1")
	SensorRegistry, @SerializedName("2")
	DataCollector, @SerializedName("3")
	SensorObserver, @SerializedName("4")
	ServiceObserver
}
