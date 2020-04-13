/*
insert into temp_table (id, name, date, cases, death, recovered)
SELECT id, name, '31/03', 0, 0, 0 from District

*/

/*
insert into DailyData (DistrictId, date, cases, death, recovered)
select id, date, cases, death, recovered from temp_table
*/

/*
DELETE from temp_table
*/