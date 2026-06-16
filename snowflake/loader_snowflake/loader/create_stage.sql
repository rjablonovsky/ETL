use GLJ_TEST.pgsdeprod;

create or replace stage pg13waretest_stage
copy_options = (on_error='continue')
file_format = (type = 'CSV' field_delimiter = ',' skip_header = 0);