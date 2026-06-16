select count(*) as row_count, 'PGSDEPROD.GEOBASE_ABORIGINAL_LANDS' as table_name from PGSDEPROD.GEOBASE_ABORIGINAL_LANDS union all
select count(*) as row_count, 'PGSDEPROD.GLJ_WELL_DIR_SRVY_LOC' as table_name from PGSDEPROD.GLJ_WELL_DIR_SRVY_LOC union all
select count(*) as row_count, 'PGSDEPROD.GLJ_WLD_WELL_BH' as table_name from PGSDEPROD.GLJ_WLD_WELL_BH
;
