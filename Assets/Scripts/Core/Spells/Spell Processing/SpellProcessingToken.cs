using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public sealed class SpellProcessingToken
    {
        public readonly List<(ulong, float)> ProcessingEntries = new List<(ulong, float)>();
        public Vector3 Destination { get; internal set; }
        public Vector3 Source { get; internal set; }

        public SpellProcessingToken() { }

        /// <summary>Rebuilds a token from replicated launch data (client-side); fill ProcessingEntries after.</summary>
        public SpellProcessingToken(Vector3 source, Vector3 destination)
        {
            Source = source;
            Destination = destination;
        }
    }
}
