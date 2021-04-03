#!/bin/bash

url="172.19.0.4:5000/sensor/registry/getSensors"
if [ "$#" -eq 1 ]
then 
	url=$1
fi

echo "Using ip: $url"

curl "$url/sensor/registry/getSensors" | python -m json.tool; echo

