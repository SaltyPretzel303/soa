#!/bin/bash

# this script will read header of file specified by the path provided
# in the variable single_csv and generate C# class with the string fields.
# each filed corresponds to one column of csv file

# before running this script run preprocess_data.sh

single_csv="/home/nemanja/workspace/soa/SensorService/data/user_0.csv" 

header=$(head --lines 1 "$single_csv")

IFS="," # sperator

read -ra array <<< "$header"

dest_file="SensorValues.cs"
echo "public class SensorValues {" > "$dest_file"

for col in "${array[@]}"
do

	if [ "$col" == "timestamp" ]
	then
		echo "public long $col { get; set; }" >> "$dest_file"
	else
		echo "public string $col { get; set; }" >> "$dest_file"
	fi

done

echo "}" >> "$dest_file"
