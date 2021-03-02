#!/usr/bin/python3

import json  # used to parse and prettyPrint events
import sys
import pika  # rabbitmq client

broker_address = "localhost"
broker_port = 5672
lifetime_topic = "service_lifetime"
lifetime_filter = "random"

lifetime_event = "{\"eventStage\":1,\"customMessage\":\"\",\"source\":\"B42E99C14DDA\",\"time\":\"2021-03-01T02:37:54.9544276+01:00\"}"

print(f"Connecting with: {broker_address}:{broker_port}")

connection = pika.BlockingConnection(pika.ConnectionParameters(host=broker_address,
                                                               port=broker_port))
channel = connection.channel()

channel.exchange_declare(lifetime_topic,
                         "topic",
                         durable=True,
                         auto_delete=True,
                         arguments=None)

channel.basic_publish(exchange=lifetime_topic,
                      routing_key=lifetime_filter,
                      body=lifetime_event)

print("Message sent ... ")

connection.close()
