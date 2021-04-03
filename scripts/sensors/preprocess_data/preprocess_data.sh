#!/bin/bash

# this script will replace every ':' and '-' character with '_' in every header column

cd "/home/nemanja/workspace/soa/SensorService/data" 

for file in *
do 
	grep_res=$(echo "$file" | grep .csv)

	if [ "$grep_res" == "" ]
	then
		# Skip files without .csv extension
		echo "Skip: $file "
		continue
	fi

	all_line=$(cat "$file" | wc --lines)
	data_line=$((all_line-1))

	header=$(head --lines 1 "$file")
	data=$(tail --lines "$data_line" "$file")

	new_header=$(echo "$header" | sed 's/:/_/'g | sed 's/-/_/'g)

	echo "$new_header" > "$file" # write header line
	echo "$data" >> "$file" # write the rest of the data (actual values)

	echo "$file -> updated "
done