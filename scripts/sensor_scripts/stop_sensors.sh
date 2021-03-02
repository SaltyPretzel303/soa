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

name_regex=$name_prefix"*"
sensor_ids=$(docker ps --filter name="$name_regex" -q)

if [ "$sensor_ids" == "" ]
then
	echo "Not found any running sensor matching given sensor_name_prefix: " $name_prefix
	exit 
fi

for single_id in $sensor_ids
do
		single_name=$(docker inspect $single_id --format '{{.Name}}')
		single_status=$(docker inspect $single_id --format '{{.State.Status}}')
		echo $single_id " ---> " $single_name " ---> " $single_status
		docker stop $single_id
done

echo # new line

for single_id in $sensor_ids
do
		single_name=$(docker inspect $single_id --format '{{.Name}}')
		single_status=$(docker inspect $single_id --format '{{.State.Status}}')
		echo $single_id " ---> " $single_name " ---> " $single_status
done
