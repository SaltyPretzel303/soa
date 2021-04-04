#!/bin/bash

if [ "$#" -ne "1" ]
then 
	echo "Please provide source file as the first arg ... "
	exit 1
fi

echo "NOTE source file will be permanently changed !!!"
echo "Press enter to continue ... "
read a # just wait for 'enter'

source_file=$1

sed 's/"/\\"/'g "$source_file" > "./temp_output" # replace each " with \"
# g will enable replacing multiple characters on the same line
# without g only the first occurrence will be replaced 
cat ./temp_output > $source_file # put commnad output back to the original file

tr -d '\040\011\012\015' < $source_file > ./temp_output # remove every empty space
# this will remove spaces, tabs, carriage return and newlines
# tr -d "\n" < $source_file > ./dest_file - this format is also valid
cat ./temp_output > $source_file

rm -r ./temp_output
