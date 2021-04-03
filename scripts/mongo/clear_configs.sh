#!/bin/bash

mongo soa_collector_dev --eval '
	db.Configs.delete({}).forEach(printjson)
'
