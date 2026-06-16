using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Client.BehaviorGraph
{
    [UsedImplicitly, CreateAssetMenu(fileName = "MeteorCorridorActionSettings", menuName = "Game Data/Scripted Spells/Meteor Corridor", order = 1)]
    public class MeteorCorridorActionSettings : ScriptableObject
    {
        public List<MeteorCorridorAction.AttackEntry> AttackEntries;
    }
}
