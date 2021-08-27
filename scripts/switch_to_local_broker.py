import json
import sys

config_paths = [
    "../SensorService/service_config.json",
    "../CollectorService/service_config.json",
    "../SensorRegistry/service_config.json",
    "../Observers/ServiceObserver/service_config.json",
    "../Observers/DataObserver/service_config.json",
]

new_address = "soa-broker"

for config_file in config_paths:
    print("Changing config file: " + config_file)

    json_config = None
    with open(config_file) as config_txt:
        json_config = json.load(config_txt)

        production_part = json_config["Production"]
        production_part["brokerAddress"] = new_address

    # print(json.dumps(json_config, indent=4))

    with open(config_file, "w") as output_file:
        json.dump(json_config, output_file, indent=4)
