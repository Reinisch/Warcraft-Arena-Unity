using UdpKit;

namespace Core
{
    public class Player : Unit
    {
        public new class CreateToken : Unit.CreateToken
        {
            public string PlayerName { get; set; }

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
                player.Name = PlayerName;
            }
        }

        private CreateToken createToken;
        private IPlayerState playerState;
        private string playerName;

        internal override bool AutoScoped => true;

        public override string Name { get => playerName; protected set => playerState.PlayerName = playerName = value; }
        public int SpecId { get; } = 1;

        public override void Attached()
        {
            playerState = entity.GetState<IPlayerState>();

            base.Attached();

            createToken = (CreateToken)entity.AttachToken;
            createToken.Attached(this);

            if (!IsOwner)
                playerState.AddCallback(nameof(playerState.PlayerName), OnPlayerNameChanged);
        }

        public override void Detached()
        {
            createToken = null;
            playerState = null;

            base.Detached();
        }

        public override void Accept(IUnitVisitor visitor) => visitor.Visit(this);

        public override void Accept(IVisitor visitor) => visitor.Visit(this);

        public void SetTarget(Unit target)
        {
            UpdateTarget(newTarget:target);
        }

        internal void ProcessCreation()
        {
            ModifyDeathState(DeathState.Alive);

            SetHealth(MaxHealth);
        }

        private void OnPlayerNameChanged()
        {
            Name = playerState.PlayerName;
        }
    }
}