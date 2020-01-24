create table Sources.Magazine
(
	Id int not null
		constraint PK_Magazine
			primary key,
	Name nvarchar(128) not null,
	SubName nvarchar(512)
)
