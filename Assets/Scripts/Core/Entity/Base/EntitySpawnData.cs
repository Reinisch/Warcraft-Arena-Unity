using UnityEngine;

namespace Core
{
    public class EntitySpawnData
    {
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }

        public EntitySpawnData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
