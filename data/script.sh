#!/bin/bash

counter=0

for file in * 
do 
	extension=${file##*.}
	
	mv $file "$file.csv"
		
done
