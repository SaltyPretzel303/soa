#!/bin/bash

#b.Sensors.aggregate([{$match:{sensor_name:"sensor_55"}},{ $project: {records: 1}}]).pretty()

#!/bin/bash

#mongo soa --eval 'db.Sensors.aggregate([{ $match: {sensor_name: "sensor_55"} }, { $project : { records : { $filter : { input: "$records",as: "record", 
#cond: {$gte: ["$$record.timestamp",1439324357]} }  }} }]).forEach(printjson)'

mongo soa --eval 'db.Sensors.aggregate([
			{$match: {sensor_name: {$regex:"sensor_55"}}},
			{$project: {records: { 
									$filter: {
										input: "$records",
										as: "record",
										cond: {$and: [
													
														{$gte: [{$convert : {input: "$$record.timestamp", to: "double"}},NumberLong(22)]},
														{$lte: [{$convert : {input: "$$record.timestamp", to: "double"}},NumberLong(144362986500)]}
														
														
													]}
									}
								},sensor_name:1
						}
			}
			
			]).forEach(printjson)'
