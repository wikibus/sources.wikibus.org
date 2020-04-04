create table Sources.Wishlist
(
    Id int not null identity
        constraint Wishlist_pk
            primary key nonclustered,
    [User] nchar(30),
    SourceId int not null
        constraint Wishlist_Source_Id_fk
            references Sources.Source,
    Done bit not null default(0),

    constraint UQ_User_Source UNIQUE ([User], SourceId)
)
