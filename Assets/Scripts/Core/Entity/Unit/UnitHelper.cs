using UnityEngine;

namespace Core
{
    public static class UnitHelper
    {
        public const float ContactDistance = 0.5f;
        public const float InteractionDistance = 5.0f;
        public const float AttackDistance = 5.0f;
        public const float InspectDistance = 28.0f;
        public const float TradeDistance = 11.11f;
        public const float SightRangeUnit = 50.0f;
        public const float DefaultVisibilityDistance = 90.0f; // default visible distance, 90 yards on continents
        public const float DefaultVisibilityInstance = 170.0f; // default visible distance in instances, 170 yards
        public const float DefaultVisibilityBgarenas = 533.0f; // default visible distance in BG/Arenas, roughly 533 yards

        public const float DefaultWorldObjectSize = 0.388999998569489f;

        public const float DefaultCombatReach = 1.5f;
        public const float MinMeleeReach = 2.0f;
        public const float NominalMeleeRange = 5.0f;
        public const float MeleeRange = (NominalMeleeRange - MinMeleeReach * 2);

        public const int MaxPetStables = 4;
        public const float PetFollowDist = 1.0f;
        public const float PetFollowAngle = Mathf.PI / 2;

        public const int MaxFactionRelations = 4;

        public const int MaxReactive = 3;
        public const int SummonSlotPet = 0;
        public const int SummonSlotTotem = 1;
        public const int MaxTotemSlot = 5;
        public const int SummonSlotMinipet = 5;
        public const int SummonSlotQuest = 6;
        public const int MaxSummonSlot = 7;
        public const int MaxGameEntitySlot = 4;
        public const int MaxSheathState = 3;

        public const int MaxTrainerspellAbilityReqs = 3;
        public const int CurrentFirstNonMeleeSpell = 1;
        public const int CurrentMaxSpell = 4;
        public const int MaxMasterySpells = 2;
        public const int MaxSpellCharm = 4;
        public const int MaxSpellVehicle = 6;
        public const int MaxSpellPossess = 8;
        public const int MaxSpellControlBar = 10;
        public const int MaxEquipmentItems = 3;

        public const int MaxAggroResetTime = 10;
        public const float MaxAggroRadius = 45.0f;

        public const int MaxClasses = 13;
        public const int MinSpecializationLevel = 10;
        public const int MaxSpecializations = 4;
        public const int PetSpecOverrideClassIndex = MaxClasses;

        public const int AttackDisplayDelay = 200;
        public const float MaxPlayerStealthDetectRange = 30.0f;

        public const int CreatureRegenInterval = 2 * TimeHelper.InMilliseconds;
        public const int MaxKillCredit = 2;
        public const int MaxCreatureModels = 4;
        public const int MaxCreatureNames = 4;
        public const int CreatureMaxSpells = 8;
        public const int MaxCreatureDifficulties = 3;
        public const int MaxOutfitItems = 24;

        public const int MaxTrainerType = 4;
        public const int CreatureTypemaskDemonOrUndead = (1 << ((int)CreatureType.Demon - 1)) | (1 << (int)(CreatureType.Undead - 1));
        public const int CreatureTypemaskHumanoidOrUndead = (1 << ((int)CreatureType.Humanoid - 1)) | (1 << (int)(CreatureType.Undead - 1));
        public const int CreatureTypemaskMechanicalOrElemental = (1 << ((int)CreatureType.Mechanical - 1)) | (1 << (int)(CreatureType.Elemental - 1));
    }
}