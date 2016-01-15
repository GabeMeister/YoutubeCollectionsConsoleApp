select 
v.title,v.publishedat,c.title,c.channelid
from videos v 
inner join channels c 
on c.channelid=v.channelid 
-- where c.YoutubeID='UC8-Th83bH_thdKZDJCrn88g'
where c.ChannelID=76;


select 
v.title,v.publishedat,c.title,c.channelid
from channels c
inner join videos v
on c.channelid=v.channelid 
-- where c.YoutubeID='UC8-Th83bH_thdKZDJCrn88g'
where c.ChannelID=76;

create index VideosTableChannelIDIndex on videos (ChannelID);

select title from videos where ChannelID=76;