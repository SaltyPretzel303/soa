#!/bin/bash

docker create 	--name t_board \
				--publish 8080:9090 \
				--publish 1883:1883 \
				--publish 5683:5683 thingsboard/tb
