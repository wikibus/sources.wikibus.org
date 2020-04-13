alter table Sources.Source
alter column [User] nvarchar(128) null

alter table Sources.Wishlist
drop UQ_User_Source

alter table Sources.Wishlist
alter column [User] nvarchar(128) not null

alter table Sources.Wishlist
add constraint UQ_User_Source UNIQUE ([User], SourceId)
