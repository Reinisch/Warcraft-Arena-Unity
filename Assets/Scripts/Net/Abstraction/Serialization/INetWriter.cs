using UnityEngine;

namespace Net
{
    /// <summary>Write side of the framework-agnostic serializer. Replaces Bolt's UdpPacket (write).</summary>
    public interface INetWriter
    {
        void WriteBool(bool value);
        void WriteByte(byte value);
        void WriteInt(int value);
        void WriteUInt(uint value);
        void WriteLong(long value);
        void WriteULong(ulong value);
        void WriteFloat(float value);
        void WriteString(string value);
        void WriteVector3(Vector3 value);
        void WriteQuaternion(Quaternion value);
        void WriteNetId(NetId value);
    }
}
