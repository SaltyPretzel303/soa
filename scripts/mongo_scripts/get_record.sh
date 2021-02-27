#!/bin/bash

mongo soa_collector --eval '
	
	db.Sensors.aggregate(
		[
			{$match: {sensor_name:"sensor_0"}},
			{$project: 
				{
					records: {$filter: 
								{
									input:"$records",
									as:"single_record",
									cond: {$eq: ["$$single_record.timestamp","1444079281"]}
								}	
							}
				}
			}
		]

	).forEach(printjson)
'
