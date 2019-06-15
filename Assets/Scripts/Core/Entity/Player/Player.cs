using JetBrains.Annotations;
using UdpKit;
using UnityEngine;

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

        [SerializeField, UsedImplicitly, Header("Player"), Space(10)] private WarcraftController controller;

        private CreateToken createToken;
        private IPlayerState playerState;
        private string playerName;

        internal GridReference<Player> GridReference { get; } = new GridReference<Player>();
        internal WarcraftController Controller => controller;

        internal override bool AutoScoped => true;

        public override string Name { get => playerName; protected set => playerState.PlayerName = playerName = value; }
        public int SpecId { get; } = 1;

        public override void Attached()
        {
            playerState = entity.GetState<IPlayerState>();

            base.Attached();

            createToken = (CreateToken)entity.attachToken;
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

        private void OnPlayerNameChanged()
        {
            Name = playerState.PlayerName;
        }
    }
}