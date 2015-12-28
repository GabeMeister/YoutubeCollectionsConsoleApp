-- select * from channels where channelid=74;
-- 
-- select count(*) from videos;

-- select * from channels where title='MLB';
-- 
-- select * from videos where channelid=34;

-- alter table Videos add PublishedAt timestamp;

-- update videos set PublishedAt='2015-12-27 08:12:35' where YoutubeID='5pfDp6Wrf6Y';


-- update videos set PublishedAt='2015-12-25 10:12:21' where YoutubeID='j5DCb1ycXyA';

-- select * from videos where YoutubeID='j5DCb1ycXyA';

select count(*) from videos where publishedat is NULL;