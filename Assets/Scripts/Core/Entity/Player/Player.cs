using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public sealed partial class Player : Unit
    {
        public new class CreateToken : Unit.CreateToken
        {
            public string PlayerName { private get; set; }

            public void Attached(Player player)
            {
                base.Attached(player);

                player.Name = PlayerName;
            }
        }

        [SerializeField, UsedImplicitly, Header(nameof(Player)), Space(10)]
        private PlayerAI playerAI;

        [SerializeField, UsedImplicitly]
        private UnitAttributeDefinition playerAttributeDefinition;

        internal VisibilityController Visibility { get; } = new();
        internal SpellController PlayerSpells { get; } = new();
        internal ClassInfo CurrentClass { get; private set; }
        internal new CreateToken CreationToken { get; private set; }

        internal override UnitAI AI => playerAI;
        internal override UnitAttributeDefinition AttributeDefinition => playerAttributeDefinition;

        public override string Name { get; internal set; }

        public ControllerInputSettings InputProvider { set => CharacterController.InputProvider = value; }

        public override void Attached(Entity.CreateToken createToken)
        {
            CreationToken = (CreateToken)createToken;

            base.Attached(createToken);
        }

        protected override void HandleAttach()
        {
            base.HandleAttach();

            CreationToken.Attached(this);
            HandleClassChange(ClassType, false);
        }

        protected override void AddBehaviours(BehaviourController unitBehaviourController)
        {
            base.AddBehaviours(unitBehaviourController);

            unitBehaviourController.TryAddBehaviour(Visibility);
            unitBehaviourController.TryAddBehaviour(PlayerSpells);
        }

        public void Accept(IUnitVisitor visitor) => visitor.Visit(this);

        public void SetTarget(Unit target)
        {
            Attributes.UpdateTarget(newTarget: target, updateState: true);
        }

        internal override void UpdateVisibility(bool forced)
        {
            base.UpdateVisibility(forced);

            if (forced)
                Map.UpdateVisibilityFor(this);
        }

        public void SwitchClass(ClassType classType)
        {
            if (ClassType != classType)
                HandleClassChange(classType, true);
        }

        // Client: a replicated class change arrived (server switched this player's class). Run the same
        // class-change path — HandleClassChange is client-safe (the spellbook build is gated to the server).
        protected override void ApplyNetworkClass(ClassType classType) => SwitchClass(classType);

        private void HandleClassChange(ClassType classType, bool isUpdate)
        {
            ClassType = classType;
            CurrentClass = Balance.ClassesByType[classType];

            // Building the spellbook casts/applies spells — server-authoritative logic. A remote client
            // recreating a replicated player must NOT run it (Entity.IsOwner is a single-player default =
            // always true, so it can't gate this yet; gate on server logic until real ownership lands).
            if (IsOwner && World.HasServerLogic)
            {
                if (isUpdate)
                    PlayerSpells.UpdateClassSpells(CurrentClass);
                else
                    PlayerSpells.AddClassSpells(CurrentClass);
            }

            Attributes.UpdateAvailablePowers();
        }
    }
}