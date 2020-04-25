WITH RowNumbers AS (
    select Id, row_number() over (PARTITION BY SourceId order by Id) as OrderIndex
    from Sources.Images
)
update images
set OrderIndex = r.OrderIndex
from Sources.Images
JOIN RowNumbers r ON images.ID = r.ID;
