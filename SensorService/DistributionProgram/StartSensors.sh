#!/bin/bash

# this script accepts three arguments:
# 1. number of sensorReader processes 
#    that are going to be started in this container
# 2. index of the first sensorReader 
#	 (index defines sensor name and processing data file)
# 3. static ip address of host/container


# default values
# this values will be used only when container is not started with start_sensor_conatiners.sh script
# (started with docker-compose or just docker run soa-sensor-service)

# max value is 60, but 5 is just enough for testing
sensor_count="5"
first_sensor_index="0"
# docker dns will translate this container name to actall ip
# container name is specified in docker-compose.yaml
host_ip="soa-test-sensor"

# overwrite default values with the ones passed from cli
if [ "$#" -eq "3" ]
then
	sensor_count="$1"
	first_sensor_index="$2"
	host_ip="$3"
fi

last_sensor_index=$(($first_sensor_index+$sensor_count-1))

# generate configs and copy bins for each sensor
./PrepareSensors.sh "$first_sensor_index" "$last_sensor_index" "$host_ip"

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


sleep_pid="-1"
shutdown_requested="0"

shutdown_sensors()
{
	if [ "${#sensor_pids[@]}" -ne "0" ]
	then

		shutdown_requested="1"

		trap '' INT TERM # ignore INT and TERM while shutting down

		for single_pid in "${sensor_pids[@]}"
		do
			echo "Killing sensor with pid: $single_pid"
			kill "$single_pid"
			wait "$single_pid"
		done

		if [ "$sleep_pid" -ne "-1" ]
		then
			kill "$sleep_pid"
		fi

	fi
}

trap shutdown_sensors SIGTERM SIGINT

dead_sensors="0"
while [ "$dead_sensors" -ne "${#sensor_pids[@]}" ] && [ "$shutdown_requested" -eq "0" ]
do

	# "healtcheck every 30s"
	sleep 30 & sleep_pid="$!"
	wait "$sleep_pid"
	sleep_pid="-1"

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

done

echo "No more live sensors ..."
echo "NOTE: last healtcheck report may be invalid if container is stopped with docker stop command"
echo "Exiting ..."
