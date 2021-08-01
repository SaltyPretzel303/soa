import pika
import threading

# NOT USED
# TBoardGateway don't know how to import it for some reason ...
# it is just implemented in the same file as RabbitConnector

class ReceiverThread(threading.Thread):
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
        threading.Thread.__init__(self)

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
            print("Exception in connection with: " + self._get_address())
            # print(e)

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
            except pika.exceptions.StreamLostError:
                # this might be pika bug but I am not sure ...
                print("Normal exception occurred while closing channel ... ")

        if self._connection is not None and self._connection.is_open:
            try:
                self._connection.close()
            except pika.exceptions.StreamLostError:
                print("Normal exception occurred while closing connection ... ")
