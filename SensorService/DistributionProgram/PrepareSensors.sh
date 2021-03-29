#!/bin/bash

dir_path="sensors"

sensors_root="/sensors"
sensor_program="/sensor_program"

first_sensor="0"
last_sensor="10"
host_ip="127.0.0.1"

if [ "$#" -eq "3" ]
then 
	first_sensor=$1
	last_sensor=$2
	host_ip=$3
fi

for (( index=$first_sensor; index<=$last_sensor; index++ ))
# for index in {0..5}
do

	echo "Preparing sensor (shell): $index"

	sensor_dest=$sensors_root"/sensor_"$index
	mkdir -p $sensor_dest
	# -p option -> create parent if needed

	cp -r $sensor_program/* $sensor_dest/

	config_dest=$sensor_dest/"service_config.json"

	python3 ./generate_config.py $index $config_dest $host_ip

	echo # new line

done
