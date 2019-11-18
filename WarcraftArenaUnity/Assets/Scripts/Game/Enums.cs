using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game
{
    public enum CharAnimState
    {
        Stand = 0,
        Walk = 1,
        WalkBackwards = 2,
        SitGroundDown = 3,
        SitGroundUp = 4,
        Jump = 5,
        Run = 6,
        SwimIdle = 7,
        SwimUp = 8,
        Swim = 9,
        SwimFast = 10,
        BackSwim = 11,
        BackSwimFast = 12,
        SideSwimRight = 13,
        SideSwimFastRight = 14,
        SideSwimLeft = 15,
        SideSwimFastLeft = 16,
        WalkBackwardsRunning = 17,
        SideWalkingRight = 18,
        SideRunningRight = 19,
        SideWalkingLeft = 20,
        SideRunningLeft = 21,
        ShuffleLeft = 22,
        ShuffleRight = 23
    }
}
