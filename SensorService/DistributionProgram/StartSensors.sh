#!/bin/bash

# this script requires two arguments:
# 1. number of sensorReader processes 
#    that is going to be started in this container
# 2. index of first sensorReader 
#	 (index defines sensor name and processing data file)

# default values
sensor_count="60"
first_sensor_index="0"

# overwrite default values with the ones passed from cli
if [ "$#" -eq "2" ]
then
	sensor_count="$1"
	first_sensor_index="$2"
fi

last_sensor_index=$(($first_sensor_index+$sensor_count-1))

# generate configs and copy bins for each sensor
./PrepareSensors.sh "$first_sensor_index" "$last_sensor_index"

if [ $? -ne 0 ]
then
	echo "Failed to prepare sensors from: $first_sensor_index to: $last_sensor_index"
	echo "Aborting ... "
	exit "$?"
fi

# save sensorReader's pids for later healt checks 
sensor_pids=()

for (( index=$first_sensor_index; index<=$last_sensor_index; index++ ))
do

	cd "/sensors/sensor_$index"
	dotnet ./SensorService.dll &

	echo "Started sensor reader index: $index pid: $!"
	sensor_pids+=("$!")

done

dead_sensors="0"

while [ "$dead_sensors" -ne "${#sensor_pids[@]}" ]
do

	echo "Sensors healtcheck ... "
	dead_sensors="0"
	# check are sensors still alive
	for single_pid in "${sensor_pids[@]}"
	do
		
		ps_output=$(ps -p "$single_pid" --no-headers)
		
		if [ "$?" -ne "0" ]
		then 
			
			echo "Sensor with pid: $single_pid died ... "
			dead_sensors=$(($dead_sensors+1))
			# sensor_pids=("${sensor_pids[@]/single_pid}")
			# prev. line should remove this sensor's pid from the list
			# but it doesn't :)
		
		fi

	done
	echo "Dead sensors: $dead_sensors"

	sleep 3

done

echo "No more live sensors"
echo "Exiting ... "