create table Sources.Source
(
    Id int not null
        constraint PK_Source
            primary key,
    SourceType nvarchar(8) not null,
    Language nvarchar(10),
    Language2 nvarchar(10),
    Pages int,
    Year smallint,
    Month tinyint,
    Day tinyint,
    Notes nvarchar(512),
    Image image,
    FileCabinet int
        constraint FK__Source__FileCabi__7B5B524B
            references Priv.FileCabinet,
    FileOffset int,
    FolderCode nvarchar(128),
    FolderName nvarchar(512),
    BookTitle nvarchar(128),
    BookAuthor nvarchar(64),
    BookISBN nchar(13),
    MagIssueMagazine int
        constraint FK__Source__MagIssue__7C4F7684
            references Sources.Magazine,
    MagIssueNumber int,
    FileMimeType nvarchar(32),
    FileContents varbinary(max),
    Url nvarchar(1024),
    FileName nvarchar(256)
)
