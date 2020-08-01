import json
import sys

# this script should generate config file based on 
# file 'config_template.json' with appropriate values for:
# - starting and ending index of sample file which is going to be read.
# - appropriate port number (all sensor services are going to be run 
# on the same container with the different port number)

# all values are calculated based on the index of sensor service
# index of sensor service is passed trought cli args

index = sys.argv[1]
destination = sys.argv[2]

print("Generating config (py): index: " + str(index) + ", on path: " + destination)

with open ("./config_template.json") as text_config:
	json_config = json.load(text_config)

	# just random hardcoded value
	port_num = 5000

	dev_part = json_config["Development"]
	prod_part = json_config["Production"]

	samples_range = {}	
	samples_range["From"] = index
	samples_range["To"] = int(index) + 1

	dev_part["sensorsRange"] = samples_range
	prod_part["sensorsRange"] = samples_range

	dev_part["listeningPort"] = int(port_num) + int(index)
	prod_part["listeningPort"] = int(port_num) + int(index)

	dev_part["sensorName"] = "sensor_" + index
	prod_part["sensorName"] = "sensor_" + index

	json_config["Development"] = dev_part
	json_config["Production"] = prod_part

	json_config["stage"] = "Production"

	with open(destination, "w") as output_txt:
		json.dump(json_config, output_txt)

