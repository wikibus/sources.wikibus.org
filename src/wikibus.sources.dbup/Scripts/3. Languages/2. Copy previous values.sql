UPDATE Sources.Source
SET Languages = CONCAT_WS(';', Language, Language2)
