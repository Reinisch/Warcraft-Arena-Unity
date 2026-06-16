using System;
using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Client
{
    [Serializable]
    public class FloatingTextController
    {
        [SerializeField, UsedImplicitly] private FloatingText floatingTextPrototype;
        [SerializeField, UsedImplicitly] private int preinstantiatedCount = 20;
        [SerializeField, UsedImplicitly] private float hitPositionSizeMultiplier = 0.5f;

        private readonly List<FloatingText> activeTexts = new List<FloatingText>();

        public void Initialize()
        {
            GameObjectPool.PreInstantiate(floatingTextPrototype.gameObject, preinstantiatedCount);
        }

        public void Deinitialize()
        {
            for (int i = activeTexts.Count - 1; i >= 0; i--)
            {
                GameObjectPool.Return(activeTexts[i], true);
                Object.Destroy(activeTexts[i]);
            }

            activeTexts.Clear();
        }

        public void SpawnMissText(UnitRenderer targetRenderer, SpellMissType missType)
        {
            FloatingText damageText = GameObjectPool.Take(floatingTextPrototype, targetRenderer.transform.position, targetRenderer.transform.rotation);
            targetRenderer.TagContainer.ApplyPositioning(damageText);
            damageText.SetMissText(missType);
            activeTexts.Add(damageText);
        }

        public void SpawnDamageText(UnitRenderer targetRenderer, int damageAmount, HitType hitType, Vector3? hitPosition)
        {
            FloatingText damageText = GameObjectPool.Take(floatingTextPrototype, targetRenderer.transform.position, targetRenderer.transform.rotation);
            targetRenderer.TagContainer.ApplyPositioning(damageText);
            float sizeMultiplier = 1;
            if (hitPosition.HasValue)
                sizeMultiplier *= hitPositionSizeMultiplier;

            if (hitPosition.HasValue)
                damageText.transform.position = hitPosition.Value;

            damageText.SetDamage(damageAmount, hitType, sizeMultiplier);

            activeTexts.Add(damageText);
        }

        public void SpawnHealingText(UnitRenderer targetRenderer, int healingAmount, bool isCrit)
        {
            FloatingText healingText = GameObjectPool.Take(floatingTextPrototype, targetRenderer.transform.position, targetRenderer.transform.rotation);
            targetRenderer.TagContainer.ApplyPositioning(healingText);
            healingText.SetHealing(healingAmount, isCrit);
            activeTexts.Add(healingText);
        }

        public void DoUpdate(float deltaTime)
        {
            for (int i = activeTexts.Count - 1; i >= 0; i--)
            {
                if (activeTexts[i].DoUpdate(deltaTime))
                {
                    GameObjectPool.Return(activeTexts[i], false);
                    activeTexts.RemoveAt(i);
                }
            }
        }
    }
}
