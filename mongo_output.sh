#!/bin/bash

#b.Sensors.aggregate([{$match:{sensor_name:"sensor_55"}},{ $project: {records: 1}}]).pretty()

mongo soa --eval 'db.Configs.update({service_name:12321},{$push: {old_configs: {
	"stage": "Development",
	"port": 5001,
	"dbAddress": "mongodb://localhost:27017",
	"dbName": "soa",
	"sensorsCollection": "Sensors",
	"recordsCollection": "Records",
	"fieldWithRecords": "records",
	"configurationBackupCollection": "Configs",
	"sensorsList": [
	  "http://localhost:5000"
	],
	"headerUrl": "/data/sensor/header",
	"dataRangeUrl": "/data/sensor/range",
	"readInterval": 3500,
	"samplePrefix": "user_",
	"brokerAddress": "localhost",
	"brokerPort": 5672,
	"collectorReportTopic": "collector_reports",
	"configurationTopic": "config",
	"targetConfiguration": "collector"
  }}},{IsUpsert:true})'

mongo soa --eval 'db.Configs.find().forEach(printjson)'

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
			
			])'
