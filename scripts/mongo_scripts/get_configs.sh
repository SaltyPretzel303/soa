#!/bin/bash

mongo soa_collector_dev --eval '
	db.Configs.find({}).forEach(printjson)
'
