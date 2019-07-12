using JetBrains.Annotations;
using UdpKit;
using UnityEngine;

namespace Core
{
    public class Creature : Unit
    {
        public new class CreateToken : Unit.CreateToken
        {
            public string CustomNameId = string.Empty;
            public float Scale = 1.0f;

            public override void Read(UdpPacket packet)
            {
                base.Read(packet);

                CustomNameId = packet.ReadString();
                Scale = packet.ReadFloat();
            }

            public override void Write(UdpPacket packet)
            {
                base.Write(packet);

                packet.WriteString(CustomNameId);
                packet.WriteFloat(Scale);
            }

            public void Attached(Creature creature)
            {
                creature.Name = CustomNameId;
                creature.transform.localScale = new Vector3(Scale, Scale, Scale);
            }
        }

        [SerializeField, UsedImplicitly, Header(nameof(Creature)), Space(10)] private CreatureDefinition creatureDefinition;

        private CreateToken createToken;
        private string customNameId;

        internal override bool AutoScoped => true;

        public override string Name { get => string.IsNullOrEmpty(customNameId) ? creatureDefinition.CreatureNameId : customNameId; internal set => customNameId = value; }

        public override void Attached()
        {
            createToken = (CreateToken)entity.AttachToken;
            createToken.Attached(this);

            base.Attached();
        }

        public override void Detached()
        {
            createToken = null;

            base.Detached();
        }

        public virtual void Accept(IUnitVisitor unitVisitor)
        {
            unitVisitor.Visit(this);
        }
    }
}