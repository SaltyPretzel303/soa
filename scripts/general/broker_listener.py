#!/usr/bin/python3

import signal  # used to trap SIGINT
import json  # used to parse and prettyPrint events
import sys
import pika  # rabbitmq client
import argparse  # util used to parse cli arguments
from os import environ  # used for reading environment variables

ENV_ADDRESS = 'BROKER_ADDRESS'
ENV_PORT = "BROKER_PORT"
ENV_TOPICS = "TARGET_TOPICS"  # comma separated values
ENV_FILTER = "TARGET_FILTER"

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

# region helper methods for resolving variables
# priority: env_variables -> cli_input -> default_values


def resolveAddress(cli_input):
    address = environ.get(ENV_ADDRESS)
    if(address is not None and address != ""):

        print("Using addres provided with ENV variable ... ")
        return address

    if (hasattr(cli_input, "broker_address") and
            (cli_input.broker_address is not None) and (cli_input.broker_address != "")):

        print("Using address provided with cli arg ... ")
        return cli_input.broker_address

    print("Using default address value ... ")
    return DEFAULT_BROKER_ADDRESS


def resolvePort(cli_input):
    port = environ.get(ENV_PORT)
    if(port is not None):

        print("Using port provided with ENV variable ... ")
        return port

    if (hasattr(cli_input, "broker_port") and
            (cli_input.broker_port is not None) and (cli_input.broker_port != "")):

        print("Using port provided with cli arg ... ")
        return cli_input.broker_port

    print("Using default port value ... ")
    return DEFAULT_BROKER_PORT


def resolveTopics(cli_input):
    topics_str = environ.get(ENV_TOPICS)
    if(topics_str is not None):

        topics_str.replace(" ", '')  # remove all spaces
        topics_arr = topics_str.split(",")

        print("Using topics provided with ENV variable ... ")
        return topics_arr

    if(hasattr(cli_input, "topic") and
       (cli_input.topic is not None) and ((cli_input.topic != ""))):

        cli_input.topic.replace(" ", "")
        topics = cli_input.topic.split(",")

        print("Using topics provided with cli arg ... ")
        return topics

    print("Using default topics ... ")
    return DEFAULT_TOPICS


def resolveFilter(cli_input):
    filter = environ.get(ENV_FILTER)
    if(filter is not None):

        print("Using filter provided with ENV variable ... ")
        return filter

    if (hasattr(cli_input, "filter") and
            (cli_input.filter is not None) and (cli_input.filter != "")):

        print("Using filter provided with cli arg ... ")
        return cli_input.filter

    print("Using default filter ... ")
    return DEFAULT_FILTER

# endregion


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

broker_address = resolveAddress(cli_input)
broker_port = resolvePort(cli_input)

print(f"Connecting with: {broker_address}:{broker_port}")

connection = pika.BlockingConnection(pika.ConnectionParameters(host=broker_address,
                                                               port=broker_port))
channel = connection.channel()

topics = resolveTopics(cli_input)
filter = resolveFilter(cli_input)

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
        txt_content = body.decode("utf-8")

        print(txt_content)  # this will print json in one line

        # next section will print json with propper formating
        # json_obj = json.loads(txt_content)
        # json_output = json.dumps(json_obj, indent=4)
        # print(f"{json_output}")

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
        print("Connection closed ... ")

    sys.exit(0)


signal.signal(signal.SIGINT, sig_handler)

# blocking call
channel.start_consuming()
