using System.Collections.Generic;
using Common;
using Core.AuraEffects;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Info Container", menuName = "Game Data/Containers/Aura Info", order = 1)]
    internal class AuraInfoContainer : ScriptableUniqueInfoContainer<AuraInfo>
    {
        [SerializeField, UsedImplicitly] private List<AuraInfo> auraInfos;

        private readonly HashSet<int> stealthAuras = new HashSet<int>();

        protected override List<AuraInfo> Items => auraInfos;

        public override void Register()
        {
            base.Register();

            foreach(AuraInfo auraInfo in auraInfos)
                for (int i = 0; i < auraInfo.AuraEffects.Count; i++)
                    if (auraInfo.AuraEffects[i] is AuraEffectInfoStealth)
                        stealthAuras.Add(auraInfo.Id);
        }

        public override void Unregister()
        {
            stealthAuras.Clear();

            base.Unregister();
        }

        public bool IsStealthAura(int auraId) => stealthAuras.Contains(auraId);

    }
}
