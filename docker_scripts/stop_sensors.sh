#!/bin/bash

name_prefix="soa-sensor-"
if [ "$#" -eq "0" ]
then 
	echo "Using default name prefix: $name_prefix"
elif [ "$#" -eq "1" ]
then 
	name_prefix=$1
	echo "Using name prefix: $name_prefix"
else
	echo "Provide only one argument:"
	echo "docker name prefix"
	echo "e.g. ./stop_sensors.sh soa-sensor-"
fi

sensors_count=$(docker ps | grep "$name_prefix" | wc --lines)

if [ "$sensors_count" -gt 0 ]
then

	for ((index=0; index<$sensors_count; index++))
	do
		echo "Stopping: $name_prefix$index"
		docker stop "$name_prefix$index"
	done

else
	echo "There is no active containers with given name prefix."
fi