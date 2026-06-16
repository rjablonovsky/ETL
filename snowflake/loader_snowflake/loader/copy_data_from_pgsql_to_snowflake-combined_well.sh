#!/bin/bash
#
#set -x # debuging

SCRIPT_FILE="$(realpath $0)"
SCRIPT_PATH="$(dirname ${SCRIPT_FILE})"
TIMESTAMP='date +%Y%m%d_%H%M%S.%N'

# extract data from postgresql
${SCRIPT_FILE}/extract_schema_data_to_csv.sh
# load dat to snowflake
${SCRIPT_FILE}/load_data_into_schema-combined_well-from_csv_files.sh

