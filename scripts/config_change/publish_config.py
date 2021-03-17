#!/usr/bin/python3

import sys
import pika  # rabbitmq client
import argparse  # util used to parse cli arguments

# parse cli arguments

parser = argparse.ArgumentParser(
    "Publish config. file to the specified topic with specified routing key. \n")

# optional arguments
parser.add_argument("--broker",
                    dest="broker_address",
                    required=False,
                    help="Default broker address is 'localhost', use this arg to change it.")
parser.add_argument("--topic",
                    dest="topic",
                    required=False,
                    help="Default config. topic is 'config', use this arg to change it.")

# required arguments
parser.add_argument("--route",
                    dest="route",
                    required=True,
                    help="SensorService route - reader; RegistryService route - registry; CollectorService route - collector; ServicObserver - service_observer")
parser.add_argument("--file",
                    dest="file",
                    required=True,
                    help="Json file containing new configuration.")

cli_input = parser.parse_args()

broker_address = "localhost"
if (hasattr(cli_input, "broker_address")
        and (cli_input.broker_address is not None)  # field is not None
        and (not cli_input.broker_address)):  # not empty string

    broker_address = cli_input.broker_address

connection = pika.BlockingConnection(pika.ConnectionParameters(broker_address))
channel = connection.channel()

exchange_name = "config"
if (hasattr(cli_input, "topic")
        and (not cli_input.topic)
        and (cli_input.topic is not None)):

    exchange_name = cli_input.topic

conf_exchange = channel.exchange_declare(exchange=exchange_name,
                                         exchange_type="topic",
                                         durable=True,
                                         auto_delete=True)

routing_key = cli_input.route

config_file = open(cli_input.file)
config_txt = config_file.read()

channel.basic_publish("config", routing_key, config_txt)

print(f"Published config: \n{config_txt}")
