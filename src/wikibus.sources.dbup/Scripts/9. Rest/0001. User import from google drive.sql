create table Sources.Users
(
    UserId nvarchar(128) not null primary key,
    DriveImportFolder nvarchar(max) null
)
