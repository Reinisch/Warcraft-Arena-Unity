using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class Player : Unit
    {
        [SerializeField, UsedImplicitly, Header("Player"), Space(10)] private WarcraftController controller;

        internal GridReference<Player> GridReference { get; } = new GridReference<Player>();
        internal WarcraftController Controller => controller;

        internal override bool AutoScoped => true;
        public override EntityType EntityType => EntityType.Player;

        public override void Accept(IUnitVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}