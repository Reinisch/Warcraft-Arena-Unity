using UnityEngine;

namespace Net
{
    /// <summary>Read side of the framework-agnostic serializer. Replaces Bolt's UdpPacket (read).</summary>
    public interface INetReader
    {
        bool ReadBool();
        byte ReadByte();
        int ReadInt();
        uint ReadUInt();
        long ReadLong();
        ulong ReadULong();
        float ReadFloat();
        string ReadString();
        Vector3 ReadVector3();
        Quaternion ReadQuaternion();
        NetId ReadNetId();
    }
}
