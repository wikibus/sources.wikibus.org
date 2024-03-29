﻿using Argolis.Hydra.Annotations;
using NullGuard;
using Vocab;

namespace Wikibus.Sources.Filters
{
    [NullGuard(ValidationFlags.None)]
    public class SourceFilters
    {
        [Variable("language")]
        [Property(Dcterms.language)]
        public string Language { get; set; }
    }
}