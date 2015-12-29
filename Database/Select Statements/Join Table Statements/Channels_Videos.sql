select 
v.title,v.thumbnail,v.publishedat,c.title
from videos v 
inner join channels c 
on c.channelid=v.channelid 
where c.title='GEazyMusicVEVO'
order by publishedat;