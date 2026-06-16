#!/bin/bash
#
# run: sudo su - postgres
# screen
# ./load_data_into_snowflake.sh load_data_into_snowflake-pgsdeprod_tables.sql pgsdeprod 
#  
#set -x # debuging

SCRIPTFILE="$(realpath $0)"
SCRIPTPATH="$(dirname ${SCRIPTFILE})"
TIMESTAMP='date +%Y%m%d_%H%M%S.%N'

SQL="${1:-$SCRIPTFILE.sql}"
SFSCHEMA="${2:-combined_well}"
SFDB="${3:-glj_test}"
SQLFILES=""
SQLFILE=""
fcount=0 # file_counter

if [[ -s "${SQL}" ]]; then SQLFILES="${SQL}";
elif [[ -s "${SCRIPTPATH}/${SQL}" ]]; then SQLFILES="${SCRIPTPATH}/${SQL}";
else SQLFILES="${SCRIPTFILE}.sql"; echo "${SQL}" > "${SQLFILES}";
fi

LOG="${4:-${SQLFILES}.log}"

SNOWSQL_CMD='snowsql -c "${SFDB}" -s "${SFSCHEMA}" -f "${SQLFILE}" -o timing=true >> "${LOG}" 2>&1'

echo "$(eval $TIMESTAMP), START load data into snowflake: $SCRIPTFILE" | tee -a ${LOG}
fcount=0
IFS=$'\n'; for SQL in $(grep -v -e '^#' ${SQLFILES} | tr -d '\r'); do
  (( fcount = fcount + 1 ))
  if [[ -s "${SQL}" ]]; then SQLFILE="${SQL}";
  elif [[ -s "${SCRIPTPATH}/${SQL}" ]]; then SQLFILE="${SCRIPTPATH}/${SQL}";
  else SQLFILE="${SCRIPTFILE}.sql_${fcount}"; echo "${SQL}" > "${SQLFILE}";
  fi

  eval "${SNOWSQL_CMD}"
  echo "$(eval $TIMESTAMP), END load: ${SQLFILE} by $SCRIPTFILE" | tee -a ${LOG}
done
echo "$(eval $TIMESTAMP), END load data into snowflake: $SCRIPTFILE" | tee -a ${LOG}
# get first 10 error messages from the log
ERROR_MSG="$(grep -A 2 -B 2 -i -wE 'error|fail|fatal' ${LOG} | head)"
if [[ ! -z "${ERROR_MSG// /}" ]]; then echo "ERROR: ${ERROR_MSG}"; echo "LOG: $(hostname -f):${LOG} "; fi

