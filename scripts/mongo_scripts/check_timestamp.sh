#!/bin/bash

if [ "$#" -ne 1 ]
then 
	echo "Please provide 'timestamps' source as the first arg ... "
	exit 1
fi


timestamps_file="$1"
counter=0
prev_timestamp=-1


# this part will remove "timestamp" : and the " at the ond of each line
# leaving only the timestamps as numbers at each line
sed 's/"timestamp" : "/ /' "$timestamps_file" > "./temp_timestamp"
sed 's/"/ /' "./temp_timestamp" > "$timestamps_file"
rm "./temp_timestamp"
# cat "$timestamps_file"

while read -r timestamp
do 

	printf "Checking ($counter) $prev_timestamp -> $timestamp: "
	
	if [ "$prev_timestamp" -eq "-1" ]
	then
	
		printf " - OK - "
	
	elif [ "$prev_timestamp" -ge "$timestamp" ]
	then
	
		printf " - Error - \n" 
		exit 1
		
	else
		
		printf " - OK - "
		
	fi

	prev_timestamp="$timestamp"
	counter=$((counter+1))

	printf "\n"

done < "$timestamps_file"
