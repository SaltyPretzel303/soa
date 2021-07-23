package broker.messages.outgoing;

import com.google.gson.annotations.SerializedName;

public enum CollectorTelemetryType {
	@SerializedName("0")
	Access, @SerializedName("1")
	Pull
}
