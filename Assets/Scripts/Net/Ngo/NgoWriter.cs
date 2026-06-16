using Net;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Net.Ngo
{
    /// <summary>
    /// <see cref="INetWriter"/> over NGO's <see cref="FastBufferWriter"/>. Owns the buffer; dispose after
    /// sending. The <see cref="Buffer"/> is exposed so the bus can hand it to the messaging manager.
    /// </summary>
    internal sealed class NgoWriter : INetWriter, System.IDisposable
    {
        public FastBufferWriter Buffer;

        public NgoWriter(int initialSize, int maxSize)
        {
            Buffer = new FastBufferWriter(initialSize, Allocator.Temp, maxSize);
        }

        public void WriteBool(bool value) => Buffer.WriteValueSafe(value);
        public void WriteByte(byte value) => Buffer.WriteValueSafe(value);
        public void WriteInt(int value) => Buffer.WriteValueSafe(value);
        public void WriteUInt(uint value) => Buffer.WriteValueSafe(value);
        public void WriteLong(long value) => Buffer.WriteValueSafe(value);
        public void WriteULong(ulong value) => Buffer.WriteValueSafe(value);
        public void WriteFloat(float value) => Buffer.WriteValueSafe(value);
        public void WriteString(string value) => Buffer.WriteValueSafe(value ?? string.Empty);
        public void WriteVector3(Vector3 value)
        {
            Buffer.WriteValueSafe(value.x);
            Buffer.WriteValueSafe(value.y);
            Buffer.WriteValueSafe(value.z);
        }

        public void WriteQuaternion(Quaternion value)
        {
            Buffer.WriteValueSafe(value.x);
            Buffer.WriteValueSafe(value.y);
            Buffer.WriteValueSafe(value.z);
            Buffer.WriteValueSafe(value.w);
        }
        public void WriteNetId(NetId value) => Buffer.WriteValueSafe(value.Value);

        public void Dispose() => Buffer.Dispose();
    }
}
