#!/bin/bash

# mongo soa_collector_dev --eval '
# 	db.Sensors.update(
# 		{
# 			sensor_name: "sensor_0",
# 			"records.timestamp": "1444079281"
# 		},
# 		{
# 			$set: {"records.$.raw_acc:magnitude_stats:mean": "-0.123"}
# 		}
# 	)
# '

mongo soa_collector_dev --eval '
	db.Sensors.aggregate([
		{
			$match:{ sensor_name: "sensor_0", "records.timestamp": "1444079281" }
		},
		{
			$set: {"records.$.raw_acc:magnitude_stats:mean": "-0.2312321"}
		}
		]
	)
'