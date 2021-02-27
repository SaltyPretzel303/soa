#!/bin/bash

prefix="soa-sensor"
regex_match=$prefix"*"

running_ids=$(docker ps --filter name=$regex_match -q)
if [ "$running_ids" != "" ]
then 

	echo "Next containers are still running: "
	echo 

	for single_id in $running_ids
	do

		single_name=$(docker inspect $single_id --format '{{.Name}}')
		single_status=$(docker inspect $single_id --format '{{.State.Status}}')
		echo $single_id " ---> " $single_name " ---> " $single_status

	done

	echo 
	echo "Press ENTER to STOP them or CTRL+C to abort ... "
	read

	for single_id in $running_ids
	do
		
		single_name=$(docker inspect $single_id --format '{{.Name}}')
		single_status=$(docker inspect $single_id --format '{{.State.Status}}')
		docker stop $single_id
		
		echo "Stopped: " $single_id "( " $single_name " )"

	done

else
	echo "Not found any running conatiner ... "
fi

old_ids=$(docker ps -a --filter name=$regex_match -q)
# -q will return only container ids

if [ "$old_ids" != "" ]
then
	echo 
	echo "Found next old containers: "

	for single_id in $old_ids
	do

		single_name=$(docker inspect $single_id --format '{{.Name}}')
		single_status=$(docker inspect $single_id --format '{{.State.Status}}')
		echo $single_id " ---> " $single_name " ---> " $single_status

	done

	echo
	echo "Press ENTER to REMOVE them or CTRL+C to abort ... "
	read

	for single_id in $old_ids
	do

		single_name=$(docker inspect $single_id --format '{{.Name}}')
		single_status=$(docker inspect $single_id --format '{{.State.Status}}')
		echo "Removing: " $single_id "( " $single_name " )"
		docker rm $single_id

	done

else
	echo "No old containers matching: " $regex_match " ..."
fi
echo