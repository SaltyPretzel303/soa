#!/bin/bash

pid_file="$1"

while read -r pid_line
do 

	echo "Shutting down: $pid_line"
	kill "$pid_line"

done < "$pid_file"
