create trigger Sources.RemoveLegacyImage
ON Sources.Images
after INSERT
AS
BEGIN
    update Sources.Source
    SET Sources.Source.Image = null
    FROM inserted
    WHERE Sources.Source.Id = inserted.SourceId
end
