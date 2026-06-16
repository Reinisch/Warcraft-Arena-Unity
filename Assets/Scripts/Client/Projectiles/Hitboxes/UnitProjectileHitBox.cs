using System;
using Client;
using UnityEngine;

namespace Core
{
    public class UnitProjectileHitBox: MonoBehaviour
    {
        [field: SerializeField]
        public UnitModel Model { get; private set; }

        [field: SerializeField]
        public Collider HitBoxCollider { get; private set; }

        [field: SerializeField]
        public float DamageMultiplier { get; set; } = 1;

        public event Action<int> EventDamageReceived;

        public void ReceiveDamage(int damage)
        {
            EventDamageReceived?.Invoke(damage);
        }
    }
}