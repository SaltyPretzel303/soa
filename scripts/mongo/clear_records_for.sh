mongo soa_collector_dev --eval '
	
	db.Sensors.update(
		
		{sensor_name: "sensor_0"},
		{$unset: {records:""}}	

	)

'
