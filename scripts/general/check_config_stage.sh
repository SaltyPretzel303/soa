#!/bin/bash

cd ../../
root_path="`pwd`"
echo "Starting search at $root_path"
echo "Press ENTER to continue ... "
read

config_files=`find $root_path -iname "service_config.json" | grep --invert-match "bin"`

for single_file in $config_files
do
	stage=`grep -i "stage" "$single_file"`
	printf "%80s \t-> %s \n" "$single_file" "$stage"
	# echo -e "config: $single_file \t\t \a-> $stage"
	# -e will enable interpretation of escape characters
done

