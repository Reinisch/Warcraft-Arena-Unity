using System;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class SpellTooltipArgumentSettings
    {
        [SerializeField, UsedImplicitly] private ScriptableObject argumentSource;
        [SerializeField, UsedImplicitly] private SpellTooltipArgumentType argumentType;

        public float? Resolve()
        {
            float? result;
            switch (argumentSource)
            {
                case SpellInfo spellInfo:
                    result = Resolve(spellInfo);
                    break;
                case SpellEffectInfo spellEffectInfo:
                    result = Resolve(spellEffectInfo);
                    break;
                case AuraInfo auraInfo:
                    result = Resolve(auraInfo);
                    break;
                case AuraEffectInfo auraEffectInfo:
                    result = Resolve(auraEffectInfo);
                    break;
                default:
                    return null;
            }

            return result;
        }

        private float? Resolve(SpellInfo spellInfo)
        {
            switch (argumentType)
            {
                default:
                    return null;
            }
        }

        private float? Resolve(SpellEffectInfo spellEffectInfo)
        {
            switch (argumentType)
            {
                case SpellTooltipArgumentType.Value:
                    return spellEffectInfo.Value;
                default:
                    return null;
            }
        }

        private float? Resolve(AuraInfo auraInfo)
        {
            switch (argumentType)
            {
                case SpellTooltipArgumentType.Duration:
                    return (float) auraInfo.MaxDuration / 1000;
                default:
                    return null;
            }
        }

        private float? Resolve(AuraEffectInfo auraEffectInfo)
        {
            switch (argumentType)
            {
                case SpellTooltipArgumentType.Value:
                    return auraEffectInfo.Value;
                default:
                    return null;
            }
        }

#if UNITY_EDITOR
        public bool Validate()
        {
            switch (argumentSource)
            {
                case SpellInfo spellInfo:
                    return Resolve(spellInfo) != null;
                case SpellEffectInfo spellEffectInfo:
                    return Resolve(spellEffectInfo) != null;
                case AuraInfo auraInfo:
                    return Resolve(auraInfo) != null;
                case AuraEffectInfo auraEffectInfo:
                    return Resolve(auraEffectInfo) != null;
            }
            return false;
        }
#endif
    }
}
