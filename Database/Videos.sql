create table Videos (
VideoID INT PRIMARY KEY NOT NULL,
YoutubeID CHAR(15) NOT NULL,
ChannelID INT NOT NULL references Channels(ChannelID),
Title CHAR(150) NOT NULL,
Thumbnail CHAR(100),
Duration CHAR(15),
ViewCount INT
);