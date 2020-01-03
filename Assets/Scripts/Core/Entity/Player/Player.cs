using Bolt;
using JetBrains.Annotations;
using UdpKit;
using UnityEngine;

namespace Core
{
    public sealed partial class Player : Unit
    {
        public new class CreateToken : Unit.CreateToken
        {
            public string PlayerName { private get; set; }

            public override void Read(UdpPacket packet)
            {
                base.Read(packet);

                PlayerName = packet.ReadString();
            }

            public override void Write(UdpPacket packet)
            {
                base.Write(packet);

                packet.WriteString(PlayerName);
            }

            public void Attached(Player player)
            {
                base.Attached(player);

                player.Name = PlayerName;
            }
        }

        public class ControlGainToken : IProtocolToken
        {
            public bool HasMovementControl { get; set; }

            public void Read(UdpPacket packet)
            {
                HasMovementControl = packet.ReadBool();
            }

            public void Write(UdpPacket packet)
            {
                packet.WriteBool(HasMovementControl);
            }

            public void ControlGained(Player player)
            {
                player.CharacterController.UpdateMovementControl(HasMovementControl);
            }
        }

        [SerializeField, UsedImplicitly, Header(nameof(Player)), Space(10)]
        private PlayerAI playerAI;

        private CreateToken createToken;
        private IPlayerState playerState;
        private string playerName;

        internal VisibilityController Visibility { get; } = new VisibilityController();
        internal SpellController PlayerSpells { get; } = new SpellController();
        internal ClassInfo CurrentClass { get; private set; }

        internal bool IsLocalServerPlayer => IsOwner && IsController;
        internal PlayerAI PlayerAI => playerAI;
        internal override UnitAI AI => playerAI;
        internal override bool AutoScoped => false;

        public override string Name
        {
            get => playerName;
            internal set
            {
                playerName = value;

                if (IsOwner)
                {
                    playerState.PlayerName = value;
                    createToken.PlayerName = value;
                }
            } 
        }

        public IControllerInputProvider InputProvider { set => CharacterController.InputProvider = value; }

        protected override void HandleAttach()
        {
            base.HandleAttach();

            playerState = entity.GetState<IPlayerState>();
            createToken = (CreateToken)entity.AttachToken;
            createToken.Attached(this);

            HandleClassChange(ClassType, false);
            HandleStateCallbacks(true);
        }

        protected override void HandleDetach()
        {
            HandleStateCallbacks(false);

            createToken = null;
            playerState = null;

            base.HandleDetach();
        }

        protected override void HandleControlGained()
        {
            base.HandleControlGained();

            if (!IsOwner && IsController)
            {
                BoltEntity localClientMoveState = BoltNetwork.Instantiate(BoltPrefabs.Movement);
                localClientMoveState.SetScopeAll(false);
                localClientMoveState.SetScope(BoltNetwork.Server, true);
                localClientMoveState.AssignControl(BoltNetwork.Server);

                Motion.AttachMoveState(localClientMoveState);
            }

            if (BoltEntity.ControlGainedToken is ControlGainToken controlGainToken)
                controlGainToken.ControlGained(this);
        }

        protected override void HandleControlLost()
        {
            Motion.DetachMoveState(true);

            base.HandleControlLost();
        }

        protected override void AddBehaviours(BehaviourController unitBehaviourController)
        {
            unitBehaviourController.TryAddBehaviour(Visibility);
            unitBehaviourController.TryAddBehaviour(PlayerSpells);

            base.AddBehaviours(unitBehaviourController);
        }

        public void Accept(IUnitVisitor visitor) => visitor.Visit(this);

        public void SetTarget(Unit target)
        {
            Attributes.UpdateTarget(newTarget: target, updateState: World.HasServerLogic);
        }

        public void Handle(SpellPlayerTeleportEvent teleportEvent)
        {
            Position = teleportEvent.TargetPosition;
            SetMovementFlag(MovementFlags.Ascending, false);
        }

        public void Handle(PlayerSpeedRateChangedEvent speedChangeEvent)
        {
            Attributes.UpdateSpeedRate((UnitMoveType) speedChangeEvent.MoveType, speedChangeEvent.SpeedRate);
        }

        public void Handle(PlayerRootChangedEvent rootChangeEvent)
        {
            if (rootChangeEvent.Applied)
            {
                StopMoving();

                AddState(UnitControlState.Root);
            }
            else
                RemoveState(UnitControlState.Root);
        }

        public void Handle(PlayerMovementControlChanged movementControlChangeEvent)
        {
            if (movementControlChangeEvent.PlayerHasControl)
            {
                Position = movementControlChangeEvent.LastServerPosition;
                Motion.OverrideMovementFlags((MovementFlags)movementControlChangeEvent.LastServerMovementFlags);
            }

            CharacterController.UpdateMovementControl(movementControlChangeEvent.PlayerHasControl);
        }

        public bool HasClientVisiblityOf(WorldEntity target) => !World.HasServerLogic || Visibility.HasClientVisiblityOf(target);

        internal override void UpdateVisibility(bool forced)
        {
            base.UpdateVisibility(forced);

            if (forced)
                Map.UpdateVisibilityFor(this);
        }

        internal void AssignControl(BoltConnection boltConnection = null)
        {
            var controlToken = new ControlGainToken { HasMovementControl = Motion.HasMovementControl };
            if (boltConnection == null)
                BoltEntity.TakeControl(controlToken);
            else
                BoltEntity.AssignControl(boltConnection, controlToken);
        }

        internal void SwitchClass(ClassType classType)
        {
            if (ClassType != classType)
                HandleClassChange(classType, true);
        }

        private void HandleClassChange(ClassType classType, bool isUpdate)
        {
            ClassType = classType;
            CurrentClass = Balance.ClassesByType[classType];

            if (IsOwner)
            {
                if (isUpdate)
                    PlayerSpells.UpdateClassSpells(CurrentClass);
                else
                    PlayerSpells.AddClassSpells(CurrentClass);
            }

            Attributes.UpdateAvailablePowers();
        }

        private void HandleStateCallbacks(bool add)
        {
            if (IsOwner)
                return;

            if (add)
            {
                playerState.AddCallback(nameof(playerState.PlayerName), OnRemotePlayerNameChanged);
                playerState.AddCallback(nameof(playerState.ClassType), OnRemoteClassTypeChanged);
            }
            else
            {
                playerState.RemoveCallback(nameof(playerState.PlayerName), OnRemotePlayerNameChanged);
                playerState.RemoveCallback(nameof(playerState.ClassType), OnRemoteClassTypeChanged);
            }
        }

        private void OnRemotePlayerNameChanged() => Name = playerState.PlayerName;

        private void OnRemoteClassTypeChanged() => HandleClassChange((ClassType)playerState.ClassType, true);
    }
}