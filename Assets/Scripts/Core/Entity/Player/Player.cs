using UdpKit;

namespace Core
{
    public class Player : Unit
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

        private CreateToken createToken;
        private IPlayerState playerState;
        private string playerName;

        internal override bool AutoScoped => true;

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

            if (!IsOwner)
                playerState.AddCallback(nameof(playerState.PlayerName), OnPlayerNameChanged);
        }

        protected override void HandleDetach()
        {
            if (!IsOwner)
                playerState.RemoveCallback(nameof(playerState.PlayerName), OnPlayerNameChanged);

            createToken = null;
            playerState = null;

            base.HandleDetach();
        }

        public virtual void Accept(IUnitVisitor visitor) => visitor.Visit(this);

        public void SetTarget(Unit target)
        {
            Attributes.UpdateTarget(newTarget: target, updateState: World.HasServerLogic);
        }

        public void Handle(PlayerSpeedRateChangedEvent speedChangeEvent)
        {
            Attributes.UpdateSpeedRate((UnitMoveType) speedChangeEvent.MoveType, speedChangeEvent.SpeedRate);
        }

        public void Handle(PlayerRootChangedEvent rootChangeEvent)
        {
            if (rootChangeEvent.Applied)
                AddState(UnitControlState.Root);
            else
                RemoveState(UnitControlState.Root);
        }

        private void OnPlayerNameChanged()
        {
            Name = playerState.PlayerName;
        }
    }
}