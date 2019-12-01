

CREATE FUNCTION [Sources].[CompartmentUsage]
(

)
RETURNS
@sums TABLE
(
	[File] int,
	Offset int,
	SumPages int
)
AS
BEGIN
	declare @temp TABLE
(
	[File] int,
	Offset int,
	Pages int
)

insert into @temp
SELECT     f.FileCabinet, f.FileOffset, isnull(SUM(s.Pages), 0) AS sumPages
FROM         Sources.Source AS s
right JOIN Sources.Source AS f ON s.Id = f.Id
right JOIN  Priv.FileCabinet AS fC ON f.FileCabinet = fC.Id
WHERE     (fC.IsClosed = 0)
GROUP BY f.FileCabinet, f.FileOffset
ORDER BY sumPages

insert into @sums
select [File], Offset, SUM(Pages) as sumPages from @temp
group by [File], Offset
order by sumPages

return

END
go

