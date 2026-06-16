select count(*) as row_count, 'COMBINED_WELL.BUSINESS_ASSOCIATE' as table_name from COMBINED_WELL.BUSINESS_ASSOCIATE union all
select count(*) as row_count, 'COMBINED_WELL.DATA_CURRENCY' as table_name from COMBINED_WELL.DATA_CURRENCY union all
select count(*) as row_count, 'COMBINED_WELL.FIELD' as table_name from COMBINED_WELL.FIELD union all
select count(*) as row_count, 'COMBINED_WELL.PDEN_PRODUCTION_MONTH' as table_name from COMBINED_WELL.PDEN_PRODUCTION_MONTH union all
select count(*) as row_count, 'COMBINED_WELL.PDEN_SUMMARY' as table_name from COMBINED_WELL.PDEN_SUMMARY union all
select count(*) as row_count, 'COMBINED_WELL.POOL' as table_name from COMBINED_WELL.POOL union all
select count(*) as row_count, 'COMBINED_WELL.R_COUNTRY' as table_name from COMBINED_WELL.R_COUNTRY union all
select count(*) as row_count, 'COMBINED_WELL.R_PROVINCE_STATE' as table_name from COMBINED_WELL.R_PROVINCE_STATE union all
select count(*) as row_count, 'COMBINED_WELL.R_WELL_STATUS' as table_name from COMBINED_WELL.R_WELL_STATUS union all
select count(*) as row_count, 'COMBINED_WELL.STRAT_UNIT' as table_name from COMBINED_WELL.STRAT_UNIT union all
select count(*) as row_count, 'COMBINED_WELL.WELL' as table_name from COMBINED_WELL.WELL
;
