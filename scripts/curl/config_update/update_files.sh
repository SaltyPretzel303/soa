#!/bin/bash

function transform {
	raw_config="$(cat $1)"
	# echo $raw_config
	# echo $1	
	# look at ../general/stringify_json.sh script for explanation
	s1_file=$(echo "$raw_config" | sed 's/"/\\"/'g )
	s2_file=$(echo "$s1_file" | tr -d '\040\011\012\015')

	echo "$s2_file"
}

function write_in {
	content="$1"
	dest="$2"

	echo '{' > "$2"
	echo '"AdditionalData": "none",' >> "$2"
	echo '"TxtConfig":' "\"$content\"" >> "$2"
	echo '}' >> "$2"
}

cp ../../../SensorService/service_config.json ./raw_sensor_config.json
cp ../../../SensorRegistry/service_config.json ./raw_registry_config.json
cp ../../../CollectorService/service_config.json ./raw_collector_config.json
cp ../../../Observers/ServiceObserver/service_config.json ./raw_s_observer_config.json

clean_conf=$(transform "./raw_sensor_config.json")
write_in "$clean_conf" "./update_sensor.json" 

# clean_conf=$(transform "./raw_registry_config.json")
# write_in "$clean_conf" "./update_registry.json"

# clean_conf=$(transform "./raw_collector_config.json")
# write_in "$clean_conf" "./update_collector.json"

# clean_conf=$(transform "./raw_s_observer_config.json")
# write_in "$clean_conf" "./update_s_observer.json"




