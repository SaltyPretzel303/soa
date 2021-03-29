import json
import sys
import os

# will set target config to production
# used in dockerfiles trough build process

# NOT USED
# it is not THAT important and would require python installation for every
# service that would use it (that would just slow down build process ...  )

STAGE_FIELD = 'stage'
PROD_VALUE = 'Production'
DEV_VALUE = 'Development'

try:
    # argv[0] is a script's name
    config_path = sys.argv[1]
except IndexError:
    print("Please provide config file path as the only cli argument ... ")

    exit()

if not os.path.isfile(config_path):
    print("Specified file does not exists ... ")

    exit()

json_config = {}
with open(config_path) as txt_config:
    json_config = json.load(txt_config)

    if json_config[STAGE_FIELD] == DEV_VALUE:
        print("Switching to Production configuration ... ")
        json_config[STAGE_FIELD] = PROD_VALUE

    else:
        print("Configuration file already in Production stage ... ")

if json_config != {}:
    with open(config_path, "w") as config_file:
        config_file.write(json.dumps(json_config, indent=2))
