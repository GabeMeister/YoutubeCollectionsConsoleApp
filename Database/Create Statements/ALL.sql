create table Users (
UserID SERIAL PRIMARY KEY,
Username char(50)
);

create table Channels (
ChannelID SERIAL PRIMARY KEY,
YoutubeID CHAR(30) NOT NULL,
Title CHAR(150) NOT NULL,
Description CHAR(1000),
UploadPlaylist CHAR(30),
Thumbnail CHAR(100),
ViewCount BIGINT,
SubscriberCount BIGINT,
VideoCount BIGINT
);

create table Videos (
VideoID SERIAL PRIMARY KEY,
YoutubeID CHAR(15) NOT NULL,
ChannelID INT NOT NULL references Channels(ChannelID),
Title CHAR(150) NOT NULL,
Thumbnail CHAR(100),
Duration CHAR(15),
ViewCount INT
);

create table Collections (
CollectionID SERIAL PRIMARY KEY,
UserID INT NOT NULL references Users(UserID),
Title CHAR(50) NOT NULL,
Description CHAR(150)
);

create table Users_Videos (
UserVideoID SERIAL PRIMARY KEY,
UserID INT REFERENCES Users(UserID) NOT NULL,
VideoID INT REFERENCES Videos(VideoID) NOT NULL,
DateViewed char(15)
);

create table Collections_Channels (
CollectionChannelID SERIAL PRIMARY KEY,
CollectionID INT NOT NULL REFERENCES Collections(CollectionID),
ChannelID INT NOT NULL REFERENCES Channels(ChannelID)
);