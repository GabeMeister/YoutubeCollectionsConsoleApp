-- select VideoID, YoutubeID from videos where youtubeid='uhOBd34q2kM';
select VideoID, YoutubeID from videos where VideoID=143518;
select title from channels offset 557;


select ChannelID,YoutubeID,Title from Channels limit 100;

-- create index YoutubeVideoIDIndex on videos(YoutubeID);

select * from channels where Title='MrSuicideSheep';
select count(*) from channels;
select count(*) from subscriptions;
select count(*) from videos;


select * from channels where title like '%Jimmy Fallon%';


select * from videos where youtubeid='Pkm2MlXHo5A';

select * from Collections;
select * from CollectionItems;
-- select * from Channels limit 5;
