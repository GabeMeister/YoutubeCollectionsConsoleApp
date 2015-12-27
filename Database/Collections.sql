create table Collections (
CollectionID INT PRIMARY KEY NOT NULL,
UserID INT NOT NULL references Users(UserID),
Title CHAR(50) NOT NULL,
Description CHAR(150)
);