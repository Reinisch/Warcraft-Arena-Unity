using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Client.BehaviorGraph
{
    [UsedImplicitly, CreateAssetMenu(fileName = "MeteorMazeActionSettings", menuName = "Game Data/Scripted Spells/Meteor Maze", order = 1)]
    public class MeteorMazeActionSettings : ScriptableObject
    {
        public List<EffectSettings> MazeRingEffects;
    }
}