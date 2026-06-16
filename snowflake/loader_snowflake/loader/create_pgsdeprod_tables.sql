-- DROP TABLE IF EXISTS glj_wld_well_bh;
CREATE TABLE IF NOT EXISTS glj_wld_well_bh
(
    objectid integer NOT NULL,
    uwi character varying(50),
    source character varying(20),
    glj_well_id integer,
    country character varying(75),
    province_state character varying(100),
    well_name character varying(150),
    plot_symbol character varying(100),
    profile_type character varying(1),
    construction_method character varying(1),
    orig_fid integer,
    shape GEOGRAPHY
);

-- DROP TABLE IF EXISTS glj_well_dir_srvy_loc;
CREATE TABLE IF NOT EXISTS glj_well_dir_srvy_loc
(
    objectid integer NOT NULL,
    uwi character varying(20),
    survey_id character varying(10),
    province_state character varying(10),
    x_uwi_display character varying(25),
    source character varying(20),
    profile_type character varying(5),
    current_status character varying(10),
    td_strat_age integer,
    td_strat_unit_id character varying(20),
    load_date timestamp without time zone,
    shape GEOGRAPHY
);

-- DROP TABLE IF EXISTS geobase_aboriginal_lands;
CREATE TABLE IF NOT EXISTS geobase_aboriginal_lands
(
    objectid integer NOT NULL,
    acqtech character varying(23),
    credate character varying(8),
    revdate character varying(8),
    accuracy smallint,
    provider character varying(24),
    specvers character varying(10),
    alcode character varying(10),
    language1 character varying(50),
    name1 character varying(254),
    language2 character varying(50),
    name2 character varying(254),
    language3 character varying(50),
    name3 character varying(254),
    language4 character varying(50),
    name4 character varying(254),
    language5 character varying(50),
    name5 character varying(254),
    jur1 character varying(2),
    jur2 character varying(2),
    altype character varying(30),
    webref character varying(254),
    shape GEOGRAPHY
);

