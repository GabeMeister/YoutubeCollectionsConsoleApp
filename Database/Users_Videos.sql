create table Users_Videos (
UserVideoID INT PRIMARY KEY NOT NULL,
UserID INT REFERENCES Users(UserID) NOT NULL,
VideoID INT REFERENCES Videos(VideoID) NOT NULL,
DateViewed char(15)
);