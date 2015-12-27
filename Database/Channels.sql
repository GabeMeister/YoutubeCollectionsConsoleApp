create table Channels (
ChannelID INT PRIMARY KEY NOT NULL,
YoutubeID CHAR(30) NOT NULL,
Title CHAR(150) NOT NULL,
Description CHAR(1000),
UploadPlaylist CHAR(30),
Thumbnail CHAR(100),
ViewCount INT,
SubscriberCount INT,
VideoCount INT
);