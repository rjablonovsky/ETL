#!/bin/bash
# run: sudo su - postgres
# screen
# run: ./extract_data.sh sql datapath dbuser dbname dbhost linebytes log
#      sql could be command or file
#      sql structure 1: "COPY (SELECT * FROM combined_well.business_associate) TO STDOUT DELIMITER ',' CSV; -- FILE:DATAPATH/pg13waretest-combined_well-business_associate.csv"
#      sql structure 2: "COPY (SELECT * FROM combined_well.business_associate) TO '/pg/s3nfs/pg13-ware-test/loader/pg13waretest-combined_well-business_associate.csv' DELIMITER ',' CSV;"
# EXAMPLE: ./extract_data.sh "COPY (SELECT * FROM combined_well.business_associate) TO STDOUT DELIMITER ',' CSV; -- FILE:DATAPATH/pg13waretest-combined_well-business_associate.csv"
#          ./extract_data.sh extract-pg13waretest-combined_well-to_csv.sql
#          ./extract_data.sh extract-pgsdeprod-tables-to_csv.sql
#set -x # debuging


SCRIPTFILE="$(realpath $0)"
SCRIPTPATH="$(dirname ${SCRIPTFILE})"
TIMESTAMP='date +%Y%m%d_%H%M%S.%N'

SQL="${1:-extract-pg13waretest-combined_well-to_csv.sql}"
DATAPATH="${2:-/pg/s3nfs/pg13-ware-test/loader}"
DBNAME="${3:-pg13waretest}"
DBUSER="${4:-postgres}"
DBHOST="${5:-}"
LINEBYTES="${6:-100M}"
SQLFILE=""

if [[ ! -z "${DBHOST// /}" ]]; then DBHOST="-h ${DBHOST}"; fi
if [[ -s "${SQL}" ]]; then SQLFILE="${SQL}";
elif [[ -s "${SCRIPTPATH}/${SQL}" ]]; then SQLFILE="${SCRIPTPATH}/${SQL}";
else SQLFILE="${SCRIPTFILE}.sql"; echo "${SQL}" > "${SQLFILE}";
fi

LOG="${7:-${SQLFILE}.log}"


echo "$(eval $TIMESTAMP), START extract data: $SCRIPTFILE" | tee -a ${LOG}
IFS=$'\n'; for SQL in $(cat ${SQLFILE}); do
  DATAFILE=$(echo "${SQL}" | awk -F'FILE:' '{print $2}' | sed "s#DATAPATH#${DATAPATH}#g") 2>>"${LOG}"
  # export csv data to stdout and split to csv files of 100MB chunks. 60GB  ~ 30min on pg13-ware-test
  psql -X -A -w -t "${DBUSER}" -d "${DBNAME}" ${DBHOST} -c "${SQL}" 2>>"${LOG}" | \
    split --suffix-length=5 --line-bytes="${LINEBYTES}" - "${DATAFILE}." >>"${LOG}" 2>&1
  echo "$(eval $TIMESTAMP), END extract data and split to ${DATAFILE}.*: $SCRIPTFILE" | tee -a ${LOG}
done
echo "$(eval $TIMESTAMP), END extract data: $SCRIPTFILE" | tee -a ${LOG}
# get first 10 error messages from the log
ERROR_MSG="$(grep -A 2 -B 2 -i -wE 'error|fail|fatal' ${LOG} | head)"
if [[ ! -z "${ERROR_MSG// /}" ]]; then echo "ERROR: ${ERROR_MSG}"; echo "LOG: $(hostname -f):${LOG} "; fi

