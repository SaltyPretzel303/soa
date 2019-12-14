#!/bin/bash

dir_path="sensors"

sensors_root="/sensors"
sensor_program="/sensor_program"

if [ "$#" -gt "0" ]
then 
	sensors_root=$1
	sensor_program=$2
fi

for index in {0..59}
do

	echo "Generating (shell): $index"

	sensor_dest=$sensors_root"/sensor_"$index
	mkdir -p $sensor_dest
	# -p option -> create parent if needed

	cp -r $sensor_program/* $sensor_dest/

	config_dest=$sensor_dest/"service_config.json"

	dotnet ./ConfigGenerator.dll $index $config_dest

	echo # new line

done
