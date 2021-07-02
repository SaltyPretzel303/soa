#!/bin/bash

mongo soa_collector --eval '
		db.Sensors.aggregate([
			{$match: {}},
			{$project: 
				{records: {$filter: 
						{
							input: "$records", 
							as: "record", 
							cond: {
								$and:[
									{$eq: ["$$record.label_SITTING", "1"]},
									{$eq: ["$$record.label_IN_CLASS", "1"]}
								]	
							}
						}
					}
				}
			},
			{$project: {"records.label_SITTING": 1,"recods.label_IN_CLASS":1}}
		])
		.pretty()
	'
