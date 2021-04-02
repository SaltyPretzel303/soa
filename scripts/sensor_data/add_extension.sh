#!/bin/bash

counter=0

for file in * 
do 
	extension=${file##*.}
	echo "File: $file -> extension: $extension"	
#	mv $file "$file.csv"
		
done
