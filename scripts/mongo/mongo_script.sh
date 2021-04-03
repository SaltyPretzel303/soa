#!/bin/bash

# mongo soa_collector --eval '

# 	db.Sensors.aggregate(
# 		[
# 			{ 
# 				$match: {sensor_name:"sensor_0"}
# 			},
# 			{
# 				$project: {
# 					records:0	
# 				}
# 			}			
			
# 		]
	
# 	).forEach(printjson)

# '
mongo soa_collector --eval '

	db.Sensors.find({"sensor_name":"sensor_0"})
	.forEach(printjson)

'