#!/usr/bin/python3

import signal  # used to trap SIGINT
import json  # used to parse and prettyPrint events
import sys
import pika  # rabbitmq client
import argparse  # util used to parse cli arguments

DEFAULT_TOPICS = [
    "service_lifetime",
    "service_log",
    "collector",
    "sensor_registry",
    "sensor_reader"
]

DEFAULT_FILTER = "#"

DEFAULT_BROKER_ADDRESS = "localhost"
DEFAULT_BROKER_PORT = 5672

parser = argparse.ArgumentParser(
    "Listen on specific topic with specific filter.\n\
	If Topic and filter are not specified script will listen on all available topics.\n\
	Default broker adress: localhost:5672")

# optional arguments
parser.add_argument("--address",
                    dest="broker_address",
                    required=False,
                    help="Message broker address. Default: localhost")

parser.add_argument("--port",
                    dest="broker_port",
                    required=False,
                    help="Message broker port. Default: 5672")

parser.add_argument("--topic",
                    dest="topic",
                    help="Topic to listen on. Default: all available topics (hardcoded)")

parser.add_argument("--filter",
                    dest="filter",
                    help="Filter to use on specified/all topics. Default: #")

cli_input = parser.parse_args()

# resolve broker address
broker_address = DEFAULT_BROKER_ADDRESS
if (hasattr(cli_input, "broker_address") and
        (cli_input.broker_address is not None) and
        (not cli_input.broker_address)):

    broker_address = cli_input.broker_address

# resolve broker port
broker_port = DEFAULT_BROKER_PORT
if (hasattr(cli_input, "broker_port") and
        (cli_input.broker_port is not None) and
        (not cli_input.broker_port)):

    broker_address = cli_input.broker_port

print(f"Connecting with: {broker_address}:{broker_port}")

connection = pika.BlockingConnection(pika.ConnectionParameters(host=broker_address,
                                                               port=broker_port))
channel = connection.channel()

topics = DEFAULT_TOPICS
if(hasattr(cli_input, "topic") and
   (cli_input.topic is not None) and
   (not cli_input.topic)):

    topics = [cli_input.topic]

filter = DEFAULT_FILTER
if(hasattr(cli_input, "filter") and
        (cli_input.filter is not None) and
        (not cli_input.filter)):

    filter = cli_input.filter

for single_topic in topics:
    channel.exchange_declare(single_topic,
                             "topic",
                             durable=True,
                             auto_delete=True,
                             arguments=None)

    queue_result = channel.queue_declare(queue="")
    queue_name = queue_result.method.queue

    channel.queue_bind(queue_name,
                       single_topic,
                       filter)

    def event_callback(ch, method, properties, body):
        print(F"Topic: {single_topic}\nFilter: {filter}")

        txt_content = body.decode("utf-8")
        json_obj = json.loads(txt_content)
        json_output = json.dumps(json_obj, indent=4)

        print(f"{json_output}")
        print()  # just an empty line

    channel.basic_consume(queue_name,
                          event_callback)


# handling ctrl+c (SIGINT)
def sig_handler(sig, frame):

    print("\nCancellation requested ... ")

    if(channel is not None and channel.is_open):
        channel.close()
        print("Channel closed ... ")

    if(connection is not None and connection.is_open):
        connection.close()
        print("Conneciton closed ... ")

    sys.exit(0)


signal.signal(signal.SIGINT, sig_handler)

# blocking call
channel.start_consuming()
