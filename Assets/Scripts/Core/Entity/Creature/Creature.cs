using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class Creature : Unit
    {
        public new class CreateToken : Unit.CreateToken
        {
            public string CustomName { get; set; } = string.Empty;
            public int CreatureInfoId { get; set; }

            public void Attached(Creature creature)
            {
                base.Attached(creature);

                creature.Name = CustomName;
            }
        }

        [SerializeField, UsedImplicitly, Header(nameof(Creature)), Space(10)]
        private CreatureAI creatureAI;

        private string creatureName;

        internal new CreateToken CreationToken { get; private set; }
        public CreatureInfo CreatureInfo { get; private set; }

        internal override UnitAI AI => creatureAI;
        internal override UnitAttributeDefinition AttributeDefinition => CreatureInfo.Attributes;

        public override string Name { get => creatureName; internal set => creatureName = value; }

        public override void Attached(Entity.CreateToken createToken)
        {
            CreationToken = (CreateToken)createToken;
            CreatureInfo = Balance.CreatureInfoById[CreationToken.CreatureInfoId];

            base.Attached(createToken);
        }

        protected override void HandleAttach()
        {
            base.HandleAttach();

            CreationToken.Attached(this);

            Attributes.UpdateAvailablePowers();
        }

        protected override void HandleDetach()
        {
            CreatureInfo = null;

            base.HandleDetach();
        }

        public void Accept(IUnitVisitor unitVisitor)
        {
            unitVisitor.Visit(this);
        }

        public override UnitSnapshot CaptureState()
        {
            UnitSnapshot snapshot = base.CaptureState();
            snapshot.Kind = UnitSnapshotKind.Creature;
            snapshot.CreatureInfoId = CreatureInfo.Id;
            return snapshot;
        }
    }
}