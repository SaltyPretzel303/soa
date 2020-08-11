#!/bin/bash

mongo soa_collector_dev --eval '
	db.Sensors.aggregate(
		[
			{$match: {sensor_name:"sensor_0"}},
			{
				$project: {
					_id:0,
					records: 
					{
						$filter:
						{
							input:"$records",
							as:"record",
							cond:{$in:["$$record.timestamp",["1444079281","1444079341"]]}
						}
					}
				}
			}
		]
	)
'

#["1444079281","1444079341"]