using Net;
using Unity.Netcode;
using UnityEngine;

namespace Net.Ngo
{
    /// <summary>
    /// <see cref="INetReader"/> over NGO's <see cref="FastBufferReader"/> (the payload handed to a
    /// named-message handler).
    /// </summary>
    internal sealed class NgoReader : INetReader
    {
        private FastBufferReader reader;

        public NgoReader(FastBufferReader reader)
        {
            this.reader = reader;
        }

        public bool ReadBool() { reader.ReadValueSafe(out bool v); return v; }
        public byte ReadByte() { reader.ReadValueSafe(out byte v); return v; }
        public int ReadInt() { reader.ReadValueSafe(out int v); return v; }
        public uint ReadUInt() { reader.ReadValueSafe(out uint v); return v; }
        public long ReadLong() { reader.ReadValueSafe(out long v); return v; }
        public ulong ReadULong() { reader.ReadValueSafe(out ulong v); return v; }
        public float ReadFloat() { reader.ReadValueSafe(out float v); return v; }
        public string ReadString() { reader.ReadValueSafe(out string v); return v; }
        public Vector3 ReadVector3()
        {
            reader.ReadValueSafe(out float x);
            reader.ReadValueSafe(out float y);
            reader.ReadValueSafe(out float z);
            return new Vector3(x, y, z);
        }

        public Quaternion ReadQuaternion()
        {
            reader.ReadValueSafe(out float x);
            reader.ReadValueSafe(out float y);
            reader.ReadValueSafe(out float z);
            reader.ReadValueSafe(out float w);
            return new Quaternion(x, y, z, w);
        }
        public NetId ReadNetId() { reader.ReadValueSafe(out ulong v); return new NetId(v); }
    }
}
