import json
import sys

# this script should generate config file based on
# file 'config_template.json' with appropriate values for:
# - starting and ending index of sample file which is going to be read.
# - appropriate port number (all sensor services are going to be run
# on the same container with the different port number)
# - config stage field set to "Production"
# - static ip address passed as cli argument

# all values are calculated based on the index of sensor service
# index of sensor service is passed trough cli args

index = sys.argv[1]
config_dest = sys.argv[2]
host_ip = sys.argv[3]

print("Generating config (py): index: " +
      str(index) + ", on path: " + config_dest)

with open("./config_template.json") as text_config:
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

    dev_part["hostIP"] = host_ip
    prod_part["hostIP"] = host_ip

    json_config["Development"] = dev_part
    json_config["Production"] = prod_part

    json_config["stage"] = "Production"

    with open(config_dest, "w") as output_txt:
        json.dump(json_config, output_txt)
