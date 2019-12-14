#!/bin/bash

mkdir sensors
dir_path="sensors"

for index in {0..59}
do

	echo "Generating (shell): $index"

	destination=$(pwd)/$dir_path"/sensor_"$index

	echo $destination

	dotnet ./ConfigGenerator.dll $index $destination

done