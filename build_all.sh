#!/bin/bash

# UNFINISHED
# DOESN'T WORK

success_keyword="Build succeeded."

root_path="`pwd`"

projects=(
	"SensorService" 
	"SensorRegistry" 
	"CollectorService",
	"CommunicationModel",
	"Observers/ServiceObserver"
	)

for project in "${projects[@]}"
do
	echo "GONNA BUILD: " $project
	
	cd $project
	build_result="`dotnet build`"
	search_result='`grep $success_keyword "$build_result"`'

	echo $search_result

	cd $root_path

done

echo $root_path
