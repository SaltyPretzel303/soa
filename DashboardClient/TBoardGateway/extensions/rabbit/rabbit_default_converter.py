# Import base class for the converter and log ("converter.log" in logs directory).
from thingsboard_gateway.connectors.converter import Converter, log
import json


class RabbitDefaultConverter(Converter):    # Definition of class.
    def __init__(self, config):    # Initialization method
        self.__config = config	  # Saving configuration to object variable
        print("CONVERTER_CONFIG: " + json.dumps(self.__config))
        device_profile = self.__config["deviceProfile"]
        self.result_dict = {
            'deviceName': "",
            'deviceType': device_profile,
            'attributes': [],
            'telemetry': []}    # template for a result dictionary.

    # Method for data conversion from device format to ThingsBoard format.
    def convert(self, config, data: bytes):
        str_data = data.decode('utf-8')
        json_data = json.loads(str(str_data))

        sensor_id = json_data["sourceId"]

        self.result_dict['deviceName'] = sensor_id
        self.result_dict['attributes'] = []
        self.result_dict['telemetry'] = [json_data]

        # print("DICTIONARY device_profile: " + self.result_dict["deviceType"])
        # returning result dictionary after converting
        return self.result_dict
