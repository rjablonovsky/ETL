# run: sudo su - postgres
# screen
SCRIPT_FILE="$(realpath $0)"
SCRIPT_PATH="$(dirname ${SCRIPT_FILE})"
TIMESTAMP='date +%Y%m%d_%H%M%S.%N'

LOG="${1:-load_data_to-snoflake-glj_test-combined_well-tables.log}"
SNOWSQL_CMD='snowsql -c glj_test -s combined_well -f "${SQLFILE}" -o timing=true >> "${LOG}" 2>&1'

echo "$($TIMESTAMP), START load csv data into snowflake schema: $SCRIPT_FILE" | tee -a ${LOG}
SQLFILE=load_into_snowflake_internal_stage_schema-combined_well-csv_files.sql; eval "${SNOWSQL_CMD}"
echo "$($TIMESTAMP), END load csv data into internal stage: $SCRIPT_FILE" | tee -a ${LOG}
SQLFILE=truncate_schema-combined_well-tables.sql; eval "${SNOWSQL_CMD}"
echo "$($TIMESTAMP), END truncate tables in snowflake schema: $SCRIPT_FILE" | tee -a ${LOG}
SQLFILE=copy_into_schema-combined_well-from_snowflake_internal_stage_csv_files.sql; eval "${SNOWSQL_CMD}"
echo "$($TIMESTAMP), END copy data into snowflake schema form internal stage: $SCRIPT_FILE" | tee -a ${LOG}
SQLFILE=remove_from_snowflake_internal_stage_schema-combined_well-csv_files.sql; eval "${SNOWSQL_CMD}"
echo "$($TIMESTAMP), END remove csv data from snowflake internal stage: $SCRIPT_FILE" | tee -a ${LOG}
SQLFILE=test_snowflake_schema-combined_well-row_count_per_table.sql; eval "${SNOWSQL_CMD}"
echo "$($TIMESTAMP), END test snowflake schema combined_well row count per table: $SCRIPT_FILE" | tee -a ${LOG}
# get first 10 error messages from the log
ERROR_MSG="$(grep -A 2 -B 2 -i -wE 'error|fail|fatal' ${LOG} | head)"
if [[ ! -z "${ERROR_MSG// /}" ]]; then echo "ERROR: ${ERROR_MSG}"; echo "LOG: $(hostname -f):${LOG} "; fi
