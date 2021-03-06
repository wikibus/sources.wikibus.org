﻿using System;
using System.Collections.Generic;
using Nancy;
using Nancy.Responses.Negotiation;
using NullGuard;

namespace Wikibus.Sources.Nancy.Responses
{
    /// <summary>
    /// Processes binary models
    /// </summary>
    [NullGuard(ValidationFlags.None)]
    public class ByteArrayProcessor : IResponseProcessor
    {
        /// <summary>
        /// Gets nothing
        /// </summary>
        /// <value>
        /// Empty
        /// </value>
        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
        {
            get { yield break; }
        }

        /// <summary>
        /// Determines whether this instance can process the specified requested media range.
        /// </summary>
        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return new ProcessorMatch
            {
                ModelResult = model is byte[] ? MatchResult.ExactMatch : MatchResult.NoMatch,
                RequestedContentTypeResult = MatchResult.DontCare
            };
        }

        /// <summary>
        /// Processes the specified requested media range.
        /// </summary>
        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return new Response
            {
                Contents = str => str.Write(model, 0, model.Length),
                ContentType = "image/jpeg"
            };
        }
    }
}
