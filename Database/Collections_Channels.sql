create table Collections_Channels (
CollectionChannelID INT PRIMARY KEY NOT NULL,
CollectionID INT NOT NULL REFERENCES Collections(CollectionID),
ChannelID INT NOT NULL REFERENCES Channels(ChannelID)
);