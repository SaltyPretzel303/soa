#!/bin/bash

GREEN=$(tput setaf 2)
RED=$(tput setaf 1)
NORMAL=$(tput sgr0)

find_result=$( find ../../ -iname "*.csproj" )

echo "Found .netCore projects: "
echo # new line
for single_proj in $find_result
do
	echo "$single_proj"
done

echo # new line
echo "Press ENTER to build all ... "
read

for single_proj in $find_result
do
	
	build_output=$(dotnet build "$single_proj")
	# echo "$?"

	if [ "$?" -eq 0 ]
	then
		# success
	
		printf "%70s \t->\t %s\n" "$single_proj" "${GREEN}Build succeeded.${NORMAL}"
	else
		# fail 

		printf "%70s \t->\t %s\n" "$single_proj" "${RED}Build failed.${NORMAL}"
		printf "%s\n" "$build_output"

		exit 1
	fi

done

exit 0
