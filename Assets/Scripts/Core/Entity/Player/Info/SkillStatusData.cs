namespace Core
{
    public class SkillStatusData
    {
        public byte Pos;
        public SkillUpdateState UpdateState;


        public SkillStatusData(byte pos, SkillUpdateState state)
        {
            Pos = pos;
            UpdateState = state;
        }
    }
}