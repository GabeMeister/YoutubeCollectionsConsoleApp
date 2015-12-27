create table Users (
UserID int primary key not null,
Username char(50)
);

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

create table Videos (
VideoID INT PRIMARY KEY NOT NULL,
YoutubeID CHAR(15) NOT NULL,
ChannelID INT NOT NULL references Channels(ChannelID),
Title CHAR(150) NOT NULL,
Thumbnail CHAR(100),
Duration CHAR(15),
ViewCount INT
);

create table Collections (
CollectionID INT PRIMARY KEY NOT NULL,
UserID INT NOT NULL references Users(UserID),
Title CHAR(50) NOT NULL,
Description CHAR(150)
);

create table Users_Videos (
UserVideoID INT PRIMARY KEY NOT NULL,
UserID INT REFERENCES Users(UserID) NOT NULL,
VideoID INT REFERENCES Videos(VideoID) NOT NULL,
DateViewed char(15)
);

create table Collections_Channels (
CollectionChannelID INT PRIMARY KEY NOT NULL,
CollectionID INT NOT NULL REFERENCES Collections(CollectionID),
ChannelID INT NOT NULL REFERENCES Channels(ChannelID)
);