import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import broker.EventHandler;
import broker.BrokerReceiver;
import broker.handlers.CollectorAccessHandler;
import broker.handlers.CollectorPullHandler;
import broker.handlers.DefaultHandler;
import broker.handlers.RegistryEventHandler;
import broker.handlers.SensorReaderHandler;
import config.ConfigFields;
import config.EventHandlerInfo;
import config.ServiceConfiguration;

public class Launcher {

	public static void main(String[] args) {

		var config = ServiceConfiguration.getInstance();

		List<EventHandler> handlers = new ArrayList<EventHandler>();

		for (EventHandlerInfo handlerInfo : config.handlersInfo) {
			var newHandler = new DefaultHandler(
					handlerInfo.deviceType,
					handlerInfo.deviceTopic,
					handlerInfo.deviceFilter);

			handlers.add(newHandler);
		}

		var receiver = new BrokerReceiver(handlers);
		var receiverThread = new Thread(receiver);
		receiverThread.start();

		Runtime.getRuntime().addShutdownHook(new Thread() {
			@Override
			public void run() {
				if (receiver != null) {
					System.out.print("Shutting down receiver ... ");
					receiver.StopReceiver();
				}
			}
		});

		try {
			// this should keep application running
			receiverThread.join();
		} catch (InterruptedException e1) {
			e1.printStackTrace();
		}

	}

	// next two static methods were used in development
	// left just for the reference/example
	public static List<EventHandler> getDefaultHandlers(
			ConfigFields config) {

		var sensorDefRec = new broker.handlers.DefaultHandler(
				config.tBoardSensorDeviceType,
				config.sensorReaderTopic,
				config.sensorReaderFilter);
		var registryDefRec = new broker.handlers.DefaultHandler(
				config.tBoardRegistryDeviceType,
				config.registryEventTopic,
				config.registryEventFilter);
		var collectorAccessDefRec = new broker.handlers.DefaultHandler(
				config.tBoardCollectorDeviceType,
				config.collectorEventTopic,
				config.collectorAccessEventFilter);

		var collectorPullDefRec = new broker.handlers.DefaultHandler(
				config.tBoardCollectorDeviceType,
				config.collectorEventTopic,
				config.collectorPullEventFilter);

		var defaultHandlers = new ArrayList<EventHandler>();
		defaultHandlers.add(sensorDefRec);
		defaultHandlers.add(registryDefRec);
		defaultHandlers.add(collectorPullDefRec);
		defaultHandlers.add(collectorAccessDefRec);

		return defaultHandlers;
	}

	public static List<EventHandler> getSpecificHandlers(
			ConfigFields config) {
		var receiverList = new ArrayList<EventHandler>();

		var sensorDeviceType = config.tBoardSensorDeviceType;
		var sensorReaderTopic = config.sensorReaderTopic;
		var sensorReaderFilter = config.sensorReaderFilter;
		var sensorHandler = new SensorReaderHandler(
				sensorDeviceType,
				sensorReaderTopic,
				sensorReaderFilter);
		receiverList.add(sensorHandler);

		var registryDeviceType = config.tBoardRegistryDeviceType;
		var registryTopic = config.registryEventTopic;
		var registryFilter = config.registryEventFilter;
		var registryEventHandler = new RegistryEventHandler(
				registryDeviceType,
				registryTopic,
				registryFilter);
		receiverList.add(registryEventHandler);

		var collectorTopic = config.collectorEventTopic;
		var collectorDeviceType = config.tBoardCollectorDeviceType;

		var collectorAccessFilter = config.collectorAccessEventFilter;
		var collectorAccessHandler = new CollectorAccessHandler(
				collectorDeviceType,
				collectorTopic,
				collectorAccessFilter);
		receiverList.add(collectorAccessHandler);

		var collectorPullFilter = config.collectorPullEventFilter;
		var collectorPullHandler = new CollectorPullHandler(
				collectorDeviceType,
				collectorTopic,
				collectorPullFilter);
		receiverList.add(collectorPullHandler);

		return receiverList;

	}

}
