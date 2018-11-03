namespace Core
{
    public enum PetType
    {
        SummonPet = 0,
        HunterPet = 1,
    }

    public enum PetSaveMode
    {
        AsDeleted = -1,                               // not saved in fact
        AsCurrent = 0,                                // in current slot (with player)
        FirstStableSlot = 1,
        LastStableSlot = UnitHelper.MaxPetStables,   // last in DB stable slot index (including), all higher have same meaning as PET_SAVE_NOT_IN_SLOT
        NotInSlot = 100                              // for avoid conflict with stable size grow will use 100
    }

    public enum HappinessState
    {
        Unhappy = 1,
        Content = 2,
        Happy = 3
    }

    public enum PetSpellState
    {
        Unchanged = 0,
        Changed = 1,
        New = 2,
        Removed = 3
    }

    public enum PetSpellType
    {
        Normal = 0,
        Family = 1,
        Talent = 2
    }

    public enum ActionFeedback
    {
        None = 0,
        PetDead = 1,
        NothingToAtt = 2,
        CantAttTarget = 3
    }

    public enum PetTalk
    {
        SpecialSpell = 0,
        Attack = 1
    }
}