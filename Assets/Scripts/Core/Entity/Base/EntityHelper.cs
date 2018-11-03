namespace Core
{
    public class EntityHelper
    {
        public const float ContactDistance = 0.5f;
        public const float InteractionDistance = 5.0f;
        public const float AttackDistance = 5.0f;
        public const float InspectDistance = 28.0f;
        public const float TradeDistance = 11.11f;
        public const float MaxVisibilityDistance = GridHelper.SizeOfGrids;          // max distance for visible objects
        public const float SightRangeUnit = 50.0f;
        public const float DefaultVisibilityDistance = 90.0f;                       // default visible distance, 90 yards on continents
        public const float DefaultVisibilityInstance = 170.0f;                      // default visible distance in instances, 170 yards
        public const float DefaultVisibilityBgarenas = 533.0f;                      // default visible distance in BG/Arenas, roughly 533 yards

        public const float DefaultWorldObjectSize = 0.388999998569489f;             // player size, also currently used (correctly?) for any non Unit world objects
        public const float DefaultCombatReach = 1.5f;
        public const float MinMeleeReach = 2.0f;
        public const float NominalMeleeRange = 5.0f;
        public const float MeleeRange = (NominalMeleeRange - MinMeleeReach * 2);    //center to center for players
        public const float DefaultPhase = 169;

        public const int MaxGameEntityType = 52;        // sending to client this or greater value can crash client.
        public const int MaxGameEntityData = 33;        // max number of uint32 vars in gameobject_template data field

        public static int MaxCorpseType = 3;
        public static int CorpseReclaimRadius = 39;
    }
}