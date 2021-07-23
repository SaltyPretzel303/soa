package broker.handlers;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.thingsboard.rest.client.RestClient;
import org.thingsboard.server.common.data.Device;
import org.thingsboard.server.common.data.page.PageData;
import org.thingsboard.server.common.data.page.PageLink;

import config.ServiceConfiguration;

public class DeviceCache {

	private class DeviceWithToken {
		public Device device;
		public String token;

		public DeviceWithToken(Device device, String token) {
			this.device = device;
			this.token = token;
		}

	}

	private Map<String, DeviceData> cache;

	private String deviceType;

	public DeviceCache(String type) {

		this.deviceType = type;
		this.cache = new HashMap<String, DeviceData>();

		var devices = this.pullExistingDevices();

		for (DeviceWithToken singleDevice : devices) {

			var newDeviceRecord = new DeviceData(
					singleDevice.device.getId().getId(),
					"soa-id",
					singleDevice.token);

			this.cache.put(singleDevice.device.getName(), newDeviceRecord);
		}

	}

	public DeviceData getDevice(String name) {
		var reqDevice = this.cache.get(name);

		if (reqDevice == null) {
			var newDevice = this.createDevice(name);
			reqDevice = new DeviceData(
					newDevice.device.getId().getId(),
					"soa-id",
					newDevice.token);

			this.cache.put(name, reqDevice);
		}

		return reqDevice;
	}

	private List<DeviceWithToken> pullExistingDevices() {
		var config = ServiceConfiguration.getInstance();

		String url = "http://" + config.tBoardAddress + ":" + config.tBoardPort;
		String username = config.tBoardTenantUsername;
		String password = config.tBoardTenantPassword;

		var tBoardClient = new RestClient(url);
		tBoardClient.login(username, password);

		try {
			tBoardClient.getUser();

			PageData<Device> devPages = tBoardClient.getTenantDevices(
					this.deviceType,
					new PageLink(60));
			// 60 is the max number of devices possible (60 sensors)
			var retDevices = new ArrayList<DeviceWithToken>();

			for (var device : devPages.getData()) {

				var token = tBoardClient
						.getDeviceCredentialsByDeviceId(device.getId())
						.get()
						.getCredentialsId();

				retDevices.add(new DeviceWithToken(device, token));
			}

			System.out.println("Found: " + retDevices.size() +
					" devices of type: "
					+ this.deviceType);

			return retDevices;

		} catch (Exception e) {
			System.out.println("Failed to get devices of type: "
					+ this.deviceType);
			System.out.println(e.getMessage());
		} finally {
			if (tBoardClient != null) {
				tBoardClient.close();
			}
		}

		return new ArrayList<DeviceWithToken>();
	}

	private DeviceWithToken createDevice(String name) {

		System.out.println("Creating device with name: " + name);

		var config = ServiceConfiguration.getInstance();

		String url = "http://" + config.tBoardAddress + ":" + config.tBoardPort;
		String username = config.tBoardCustomerUsername;
		String password = config.tBoardCustomerPassword;

		var tBoardClient = new RestClient(url);
		tBoardClient.login(username, password);

		var customer = tBoardClient.getUser().get();

		Device newDevice = new Device();
		newDevice.setName(name);
		newDevice.setType(this.deviceType);
		newDevice.setCustomerId(customer.getCustomerId());

		tBoardClient.logout();

		String tenantUsername = config.tBoardTenantUsername;
		String tenantPassword = config.tBoardTenantPassword;

		tBoardClient.login(tenantUsername, tenantPassword);

		newDevice.setTenantId(tBoardClient.getUser().get().getTenantId());

		var savedDevice = tBoardClient.saveDevice(newDevice);

		var credentials = tBoardClient
				.getDeviceCredentialsByDeviceId(savedDevice.getId())
				.get();

		tBoardClient.logout();
		tBoardClient.close();

		return new DeviceWithToken(savedDevice, credentials.getCredentialsId());
	}

}
