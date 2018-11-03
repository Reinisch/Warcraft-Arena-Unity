namespace Core
{
    public enum SelectTargetType
    {
        Dontcare = 0, //All target types allowed

        Self, //Only Self casting

        SingleEnemy, //Only Single Enemy
        AoeEnemy, //Only AoE Enemy
        AnyEnemy, //AoE or Single Enemy

        SingleFriend, //Only Single Friend
        AoeFriend, //Only AoE Friend
        AnyFriend //AoE or Single Friend
    }

    public enum SelectEffect
    {
        Dontcare = 0, //All spell effects allowed
        Damage, //Spell does damage
        Healing, //Spell does healing
        Aura //Spell applies an aura
    }

    public enum SelectEquip
    {
        NoChange = -1,
        Unequip = 0
    }

    public enum EvadeReason
    {
        NoHostiles, // the creature's threat list is empty
        Boundary, // the creature has moved outside its evade boundary
        SequenceBreak, // this is a boss and the pre-requisite encounters for engaging it are not defeated yet
        Other
    }

    public enum Permitions
    {
        No = -1,
        Idle = 1,
        Reactive = 100,
        Proactive = 200,
        FactionSpecific = 400,
        Special = 800
    }

    public enum AITarget
    {
        Self,
        Victim,
        Enemy,
        Ally,
        Buff,
        Debuff
    }

    public enum AICondition
    {
        Aggro,
        Combat,
        Die
    }

    public enum AIReaction
    {
        Alert = 0, // pre-aggro (used in client packet handler)
        Friendly = 1, // (NOT used in client packet handler)
        Hostile = 2, // sent on every attack, triggers aggro sound (used in client packet handler)
        Afraid = 3, // seen for polymorph (when AI not in control of self?) (NOT used in client packet handler)
        Destroy = 4 // used on object destroy (NOT used in client packet handler)
    }
}