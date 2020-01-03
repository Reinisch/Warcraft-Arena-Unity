using System;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class ActionButtonData
    {
        [SerializeField, UsedImplicitly] private int actionId;
        [SerializeField, UsedImplicitly] private ButtonContentType actionType;

        public int ActionId => actionId;
        public ButtonContentType ActionType => actionType;

        public ActionButtonData() => Reset();

        public ActionButtonData(ActionButtonData other) => Modify(other.actionId, other.actionType);

        public ActionButtonData(int actionId, ButtonContentType actionType) => Modify(actionId, actionType);

        public void Reset() => Modify(0, ButtonContentType.Empty);

        public void Modify(ActionButtonData other) => Modify(other.ActionId, other.ActionType);

        public void Modify(SpellInfo spellInfo) => Modify(spellInfo.Id, ButtonContentType.Spell);

        public void Modify(int actionId, ButtonContentType actionType)
        {
            this.actionId = actionId;
            this.actionType = actionType;
        }
    }
}
