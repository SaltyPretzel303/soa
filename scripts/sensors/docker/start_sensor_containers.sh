#!/bin/bash

# default values
num_of_instances="2"
proc_per_instance="3"
memory_per_proc="90"
data_path="/home/nemanja/workspace/soa/volumes/sensor_data"

NETWORK="soa-network"
IP_SUBNET="172.23.1"

sensor_name_prefix="soa-sensor-"

# validate cli arguments
if [ "$#" -eq "0" ]
then 
	echo "Taking default values for arguments ... "
else

	if [ "$#" -ge "2" ]
	then
		num_of_instances=$1
		proc_per_instance=$2	

		if [ "$#" -gt "2" ]
		then
			data_path=$3
		fi
	else
		echo "Please provide adequate arguments:"
		echo "arg1: number of containers"
		echo "arg2: processes per instance"
		echo "arg3: absolute path to the data files"
		echo "e.g.: ./start_sensor_containers.sh 10 6 /home/nemanja/soa_data"
		echo
		exit	
	fi

fi

echo "Number of containers:  $num_of_instances "
echo "Processes per container: $proc_per_instance ... "
echo "Data should be on: $data_path "
echo # new line

# list previously used containers (if they exist)

regex_match=$sensor_name_prefix"*"
old_containers=$(docker ps -a --filter name=$regex_match -q)

if [ "$old_containers" != "" ]
then

	echo "Found next conflicted containers: "
	echo

	for single_id in $old_containers
	do

		single_name=$(docker inspect $single_id --format '{{.Name}}')
		single_status=$(docker inspect $single_id --format '{{.State.Status}}')
		echo $single_id " ---> " $single_name " ---> " $single_status

	done

	echo
	echo "Remove them before starting new ones ... "
	exit

else
	echo "No conlicting containers ... "
	echo
fi

data_index="0"

memory_per_container=$((memory_per_proc*proc_per_instance))
echo "Starting containers with max of $memory_per_container MB of ram..."

for (( container_ind=0; container_ind<$num_of_instances; container_ind++ ))
do 

	last_data_ind=$(($data_index+$proc_per_instance-1))
	echo "Starting container $sensor_name_prefix$container_ind with data: $data_index - $last_data_ind" 

	container_ip="$IP_SUBNET.$container_ind"

	docker run -d \
				--name "$sensor_name_prefix$container_ind" \
				--memory $memory_per_container"m" \
				--volume "$data_path:/data" \
				--network "$NETWORK" \
				--ip "$container_ip" \
				 soa/sensor-service "$proc_per_instance" "$data_index" "$container_ip"

	data_index=$(($data_index+$proc_per_instance))

	sleep 0.2 
	# just so that they are not started at the same time
	# and than all the read events happen at the same time also 

done
