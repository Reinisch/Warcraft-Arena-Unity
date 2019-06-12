using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class BalanceManager : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private BalanceReference reference;

        public void Initialize()
        {
            reference.Initialize();
        }

        public void Deinitialize()
        {
            reference.Deinitialize();
        }
    }
}