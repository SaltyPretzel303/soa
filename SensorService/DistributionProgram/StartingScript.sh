#!/bin/bash

pid_output=$1
echo > $pid_output

for index in {0..5}
do

	echo "Strting sensor: $index"

	cd /sensors/sensor_$index
	touch output
	dotnet SensorService.dll > output &
	echo "Started sensor [$index] with pid: $!"
	echo "$!" >> "$pid_output"

done
