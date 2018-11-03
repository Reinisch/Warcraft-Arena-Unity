namespace Core
{
    public class TrainerSpell
    {
        public uint SpellID;
        public uint MoneyCost;
        public uint ReqSkillLine;
        public uint ReqSkillRank;
        public uint ReqLevel;
        public uint[] ReqAbility;


        public TrainerSpell()
        {
            SpellID = 0;
            MoneyCost = 0;
            ReqSkillLine = 0;
            ReqSkillRank = 0;
            ReqLevel = 0;

            ReqAbility = new uint[UnitHelper.MaxTrainerspellAbilityReqs];
        }

        public bool IsCastable() { return ReqAbility[0] != SpellID; }
    }
}