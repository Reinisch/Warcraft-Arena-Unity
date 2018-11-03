namespace Core
{
    public class ConditionSourceInfo
    {
        public WorldEntity[] ConditionTargets { get; private set; }
        public Condition LastFailedCondition { get; set; }

        public ConditionSourceInfo(WorldEntity target0, WorldEntity target1 = null, WorldEntity target2 = null)
        {
            ConditionTargets = new WorldEntity[GlobalHelper.MaxConditionTargets];
            ConditionTargets[0] = target0;
            ConditionTargets[1] = target1;
            ConditionTargets[2] = target2;
            LastFailedCondition = null;
        }
    }
}