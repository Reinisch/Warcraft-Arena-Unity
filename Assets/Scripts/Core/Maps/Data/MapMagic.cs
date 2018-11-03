namespace Core
{
    /// <summary>
    /// Represents a map magic value of 4 bytes (used in versions).
    /// </summary>
    public struct MapMagic
    {
        /// <summary>
        /// Non-null terminated string.
        /// </summary>
        public string MagicString;
        /// <summary>
        /// Integer representation.
        /// </summary>
        public int MagicNumber;
    }
}