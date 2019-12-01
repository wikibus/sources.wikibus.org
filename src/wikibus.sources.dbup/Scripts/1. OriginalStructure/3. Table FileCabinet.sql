create table Priv.FileCabinet
(
	Id int identity
		constraint PK_File
			primary key,
	CompartmentsCount tinyint not null,
	CompartmentCapacity tinyint,
	IsClosed bit constraint DF_FileCabinet_IsClosed default 0 not null,
	Description nvarchar(32)
)