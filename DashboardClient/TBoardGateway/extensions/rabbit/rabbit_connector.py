"""Import libraries"""

from os import close
import pika

import time
from threading import Thread
from random import choice
from string import ascii_lowercase
# Import base class for connector and logger
from thingsboard_gateway.connectors.connector import Connector, log
from thingsboard_gateway.tb_utility.tb_utility import TBUtility

import traceback

# look at this file for explanation why is this commented
# from rabbit_receiver_thread import ReceiverThread


# Define a connector class, it should inherit from "Connector" class.
class RabbitConnector(Thread, Connector):
    def __init__(self, gateway,  config, connector_type):
        super().__init__()	  # Initialize parents classes
        # Dictionary, will save information about count received and sent messages.
        self.statistics = {'MessagesReceived': 0, 'MessagesSent': 0}
        self.__config = config
        self.__gateway = gateway
        self.__connector_type = connector_type

        # get from the configuration or create name for logs.
        self.setName(self.__config.get("name", "Custom %s connector " % self.get_name(
        ) + ''.join(choice(ascii_lowercase) for _ in range(5))))
        # Send message to logger
        log.info("Starting Custom %s connector", self.get_name())
        self.daemon = True	  # Set self thread as daemon
        self.stopped = True    # Service variable for check state
        self.connected = False	  # Service variable for check connection to device
        self.devices = {}	 # Dictionary with devices, will contain devices configurations, converters for devices and serial port objects
        self.load_converters()

        self._consumer_threads = []

        self._rabbit_connection = None
        self._rabbit_channels = []
        self._rabbit_consumers = []

        log.info('Custom connector %s initialization success.',
                 self.get_name())	 # Message to logger
        log.info("Devices in configuration file found: %s ", '\n'.join(
            device for device in self.devices))    # Message to logger

    def open(self):    # Function called by gateway on start
        self.stopped = False
        self.start()

    def get_name(self):    # Function used for logging, sending data and statistic
        return self.name

    def is_connected(self):    # Function for checking connection state
        return self.connected

    # Function for search a converter and save it.
    def load_converters(self):
        devices_config = self.__config.get('devices')
        try:
            if devices_config is not None:
                print("Devices config part loaded ... ")
                for device_config in devices_config:
                    if device_config.get('converter') is not None:
                        converter = TBUtility.check_and_import(
                            self.__connector_type, device_config['converter'])
                        self.devices[device_config['name']] = {'converter': converter(
                            device_config), 'device_config': device_config}
                    else:
                        log.error(
                            'Converter configuration for the custom connector %s -- not found, please check your configuration file.', self.get_name())
            else:
                log.error(
                    'Section "devices" in the configuration not found. A custom connector %s has being stopped.', self.get_name())
                self.close()
        except Exception as e:
            log.exception(e)

    def run(self):	  # Main loop of thread
        broker_address = self.__config.get("brokerAddress")
        broker_port = self.__config.get("brokerPort")

        # devices = self.__config.get("devices")
        for single_device_name in self.devices:
            single_device = self.devices[single_device_name]

            converter = single_device.get("converter")
            single_config = single_device.get("device_config")
            topics = single_config.get("topics")

            for single_topic in topics:

                new_consumer = ReceiverThread(
                    host_addr=broker_address,
                    host_port=broker_port,
                    topic=single_topic.get("topic"),
                    filter=single_topic.get("filter"),
                    device_config=single_config,
                    gateway=self.__gateway,
                    converter=converter,
                    just_a_name=self.get_name())

                new_consumer.start()
                self._consumer_threads.append(new_consumer)

    def close(self):  # Close connect function, usually used if exception handled in gateway main loop or in connector main loop
        for single_consumer in self._consumer_threads:
            print("Shutting down receiver thread ... ")
            single_consumer.close()
        # next lines will just throw multiple exceptions ...
        # for single_channel in self._rabbit_channels:
        #     single_channel.close()

        # self._rabbit_connection.close()

        self.stopped = True

    # Function used for processing attribute update requests from ThingsBoard

    def on_attributes_update(self, content):
        print("attribute update requested and kinda processed ... ")
        log.debug(content)

    def server_side_rpc_handler(self, content):
        pass


class ReceiverThread(Thread):
    def __init__(
            self,
            host_addr,
            host_port,
            topic,
            filter,
            device_config,
            gateway,
            converter,
            just_a_name):
        Thread.__init__(self)

        self._host_addr = host_addr
        self._host_port = host_port

        self._topic = topic
        self._filter = filter

        self._device_config = device_config
        self._gateway = gateway
        self._converter = converter
        self._just_a_name = just_a_name

        self._connection = None
        self._channel = None
        self._queue_name = ""

    def run(self):
        print("Receiver thread connecting with broker on: " + self._get_address())
        broker_params = pika.ConnectionParameters(
            host=self._host_addr,
            port=self._host_port)

        try:
            self._connection = pika.BlockingConnection(broker_params)

            if self._connection.is_open:
                print("Broker connection established with: " + self._get_address())
            else:
                print("Failed to connect with broker on: " + self._get_address())

                return

            self._channel = self._connection.channel()
            self._channel.exchange_declare(
                exchange=self._topic,
                exchange_type="topic",
                durable=True,
                auto_delete=True,
                arguments=None)
            queue_result = self._channel.queue_declare(queue="")
            self._queue_name = queue_result.method.queue

            self._channel.queue_bind(
                self._queue_name, self._topic, self._filter)
            self._channel.basic_consume(
                self._queue_name,
                self._consumer_method)

            self._channel.start_consuming()

        except Exception as e:
            print("Exception in connection with: "
                  + self._get_address()
                  + " (will happen when shutting down connection thread) ... ")
            # print(e)
            # traceback.print_exc()

    def _get_address(self):
        return str(self._host_addr + ":" + str(self._host_port))

    def _get_topic_config(self):
        return str("Topic: " + self._topic + " Filter: " + self._filter)

    def _consumer_method(self, ch, method, properties, body):
        print("Consumer received message on: " + self._get_topic_config())

        converted_data = self._converter.convert(self._device_config, body)
        # have no idea what is self.get_name() may be just for debugging ...
        self._gateway.send_to_storage(self._just_a_name, converted_data)

    def close(self):
        if self._channel is not None and self._channel.is_open:
            try:
                self._channel.close()
            except pika.exceptions.StreamLostError as se:
                # this might be pika bug but I am not sure ...
                print(
                    "Normal exception occurred while closing channel ... => " + str(se))
            except Exception as e:
                print(
                    "Some exception happened while closing channel (might actually be normal ... pika things) => " + str(e))

        if self._connection is not None and self._connection.is_open:
            try:
                self._connection.close()
            except pika.exceptions.StreamLostError as se:
                # this might be pika bug but I am not sure ...
                print("Normal exception occurred while closing connection => " + str(se))
            except Exception as e:
                print(
                    "Some exception happened while closing channel (might actually be normal ... pika things) => " + str(e))
