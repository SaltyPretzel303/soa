#!/bin/bash

url="172.19.0.4:5000/sensor/registry/getSensors"
if [ "$#" -eq 1 ]
then 
	url=$1
fi

echo "Using url: $url"

curl "$url" | python -m json.tool; echo

