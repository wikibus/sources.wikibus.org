create table Sources.Images
(
	SourceId int not null
		constraint Images_Source_Id_fk
			references Sources.Source,
	Id int not null
		constraint Images_pk
			primary key nonclustered,
	OriginalUrl nvarchar(max) not null,
	ThumbnailUrl nvarchar(max) not null,
	ExternalId nvarchar(max) not null
)
