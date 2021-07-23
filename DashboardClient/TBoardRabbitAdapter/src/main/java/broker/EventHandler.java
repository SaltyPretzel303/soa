package broker;

import java.io.IOException;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;

import com.rabbitmq.client.Consumer;
import com.rabbitmq.client.ShutdownSignalException;

import broker.handlers.DeviceCache;
import config.ServiceConfiguration;

public abstract class EventHandler implements Consumer {

	protected DeviceCache deviceCache;

	protected String topic;
	protected String filter;

	public EventHandler(String deviceType, String topic, String filter) {
		this.deviceCache = new DeviceCache(deviceType);
		this.topic = topic;
		this.filter = filter;
	}

	public String getTopicName() {
		return this.topic;
	}

	public String getFilter() {
		return this.filter;
	}

	protected void sendTelemetryData(Object telemetryData, String accessToken) {

		String strData = "";
		if (telemetryData instanceof String) {
			strData = telemetryData.toString();
		} else {
			strData = json.JsonUtil.serialize(telemetryData);
		}

		// curl -v -X POST -d "{\"temperature\": 25}"
		// $HOST_NAME/api/v1/$ACCESS_TOKEN/telemetry --header
		// "Content-Type:application/json"

		var config = ServiceConfiguration.getInstance();

		String url = "http://" + config.tBoardAddress + ":" + config.tBoardPort
				+ "/api/v1/" + accessToken
				+ "/telemetry";

		var request = HttpRequest.newBuilder(URI.create(url))
				.POST(HttpRequest.BodyPublishers.ofString(strData))
				.header("Content-type", "application/json").build();

		var httpClient = HttpClient.newHttpClient();
		try {

			HttpResponse<String> response = httpClient.send(
					request,
					HttpResponse.BodyHandlers.ofString());

			if (response.statusCode() != 200) {
				System.out.println("Failed to upload telemetry data ... ");
				System.out.println(response.headers().toString());
				System.out.println(response.body());
			}

		} catch (IOException | InterruptedException e) {
			e.printStackTrace();
		}

	}

	// methods bellow are inherited from com.rabbitmq.client.Consumer
	// they wont be implemented in any of handlers that extend this class

	@Override
	public void handleShutdownSignal(String consumerTag,
			ShutdownSignalException sig) {
	}

	@Override
	public void handleRecoverOk(String consumerTag) {
	}

	@Override
	public void handleConsumeOk(String consumerTag) {
	}

	@Override
	public void handleCancelOk(String consumerTag) {
	}

	@Override
	public void handleCancel(String consumerTag) throws IOException {
	}

}
