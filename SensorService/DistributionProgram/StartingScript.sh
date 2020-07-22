#!/bin/bash

pid_output=$1
> $pid_output # clear pid_output file

for index in {0..5}
do

	echo "Starting sensor: $index"

	cd /sensors/sensor_$index
	touch output

	dotnet SensorService.dll > output &
	
	echo "Started sensor [$index] with pid: $!"
	echo "$!" >> "$pid_output"

done
