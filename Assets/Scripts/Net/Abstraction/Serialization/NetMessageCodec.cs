using System;
using System.Collections.Generic;
using Core;

namespace Net
{
    /// <summary>
    /// Framework-neutral serialization for every <see cref="INetMessage"/>: maps a message type to a
    /// stable id and an explicit write/read pair over <see cref="INetWriter"/>/<see cref="INetReader"/>.
    /// Adapters reuse this and only provide the reader/writer + transport, so message serialization is
    /// written once for all SDKs.
    ///
    /// NOTE: ids are assigned by registration order — keep the order stable across builds, and only
    /// append new messages at the end. <see cref="UnitSpellLaunch"/> carries NetIds (the server translates
    /// the Core SpellProcessingToken on send; a client rebuilds it on receive), so it serializes here.
    /// </summary>
    public sealed class NetMessageCodec
    {
        private readonly Dictionary<Type, ushort> idByType = new Dictionary<Type, ushort>();
        private readonly Dictionary<ushort, Func<INetReader, INetMessage>> readerById = new Dictionary<ushort, Func<INetReader, INetMessage>>();
        private readonly Dictionary<Type, Action<INetWriter, INetMessage>> writerByType = new Dictionary<Type, Action<INetWriter, INetMessage>>();
        private ushort nextId = 1;

        public NetMessageCodec()
        {
            RegisterAll();
        }

        public bool TryGetId(Type type, out ushort id) => idByType.TryGetValue(type, out id);

        public bool IsRegistered(Type type) => idByType.ContainsKey(type);

        public void Write(INetWriter writer, INetMessage message) => writerByType[message.GetType()](writer, message);

        public INetMessage Read(ushort id, INetReader reader) => readerById[id](reader);

        private void Register<T>(Action<INetWriter, T> write, Func<INetReader, T> read) where T : INetMessage
        {
            ushort id = nextId++;
            idByType[typeof(T)] = id;
            writerByType[typeof(T)] = (w, m) => write(w, (T)m);
            readerById[id] = r => read(r);
        }

        private void RegisterAll()
        {
            // ---- Requests (client → server) ----
            Register<SpellCastRequest>(
                (w, m) => { w.WriteInt(m.SpellId); w.WriteInt((int)m.MovementFlags); },
                r => new SpellCastRequest(r.ReadInt(), (MovementFlags)r.ReadInt()));

            Register<SpellCastDestinationRequest>(
                (w, m) => { w.WriteInt(m.SpellId); w.WriteInt((int)m.MovementFlags); w.WriteVector3(m.Destination); },
                r => new SpellCastDestinationRequest(r.ReadInt(), (MovementFlags)r.ReadInt(), r.ReadVector3()));

            Register<SpellCastTargetingRequest>(
                (w, m) => { w.WriteInt(m.SpellId); w.WriteVector3(m.TargetingSource); w.WriteQuaternion(m.TargetingRotation); },
                r => new SpellCastTargetingRequest(r.ReadInt(), r.ReadVector3(), r.ReadQuaternion()));

            Register<SpellCastCancelRequest>(
                (w, m) => { },
                r => new SpellCastCancelRequest());

            Register<TargetSelectionRequest>(
                (w, m) => { w.WriteNetId(m.TargetId); },
                r => new TargetSelectionRequest(r.ReadNetId()));

            Register<PlayerEmoteRequest>(
                (w, m) => { w.WriteInt((int)m.EmoteType); },
                r => new PlayerEmoteRequest((EmoteType)r.ReadInt()));

            Register<PlayerChatRequest>(
                (w, m) => { w.WriteString(m.Message); },
                r => new PlayerChatRequest(r.ReadString()));

            Register<PlayerClassChangeRequest>(
                (w, m) => { w.WriteInt((int)m.ClassType); },
                r => new PlayerClassChangeRequest((ClassType)r.ReadInt()));

            // ---- Notifications (server → client) ----
            Register<SpellCastResultNotification>(
                (w, m) => { w.WriteInt(m.SpellId); w.WriteInt((int)m.Result); },
                r => new SpellCastResultNotification(r.ReadInt(), (SpellCastResult)r.ReadInt()));

            Register<SpellDamageDone>(
                (w, m) =>
                {
                    w.WriteNetId(m.CasterId); w.WriteNetId(m.TargetId); w.WriteInt(m.Damage);
                    w.WriteInt((int)m.HitType); w.WriteVector3(m.HitPosition); w.WriteBool(m.HasHitPosition);
                },
                r => new SpellDamageDone(r.ReadNetId(), r.ReadNetId(), r.ReadInt(),
                    (HitType)r.ReadInt(), r.ReadVector3(), r.ReadBool()));

            Register<SpellHealingDone>(
                (w, m) => { w.WriteNetId(m.HealerId); w.WriteNetId(m.TargetId); w.WriteInt(m.Heal); w.WriteBool(m.IsCrit); },
                r => new SpellHealingDone(r.ReadNetId(), r.ReadNetId(), r.ReadInt(), r.ReadBool()));

            Register<SpellMissDone>(
                (w, m) => { w.WriteNetId(m.CasterId); w.WriteNetId(m.TargetId); w.WriteInt((int)m.MissType); },
                r => new SpellMissDone(r.ReadNetId(), r.ReadNetId(), (SpellMissType)r.ReadInt()));

            Register<SpellPlayerTeleport>(
                (w, m) => { w.WriteVector3(m.TargetPosition); },
                r => new SpellPlayerTeleport(r.ReadVector3()));

            Register<SpellCooldownNotification>(
                (w, m) => { w.WriteInt(m.SpellId); w.WriteInt(m.CooldownTime); w.WriteInt(m.ServerFrame); },
                r => new SpellCooldownNotification(r.ReadInt(), r.ReadInt(), r.ReadInt()));

            Register<SpellChargeNotification>(
                (w, m) => { w.WriteInt(m.SpellId); w.WriteInt(m.CooldownTime); w.WriteInt(m.ServerFrame); },
                r => new SpellChargeNotification(r.ReadInt(), r.ReadInt(), r.ReadInt()));

            Register<PlayerSpeedRateChanged>(
                (w, m) => { w.WriteInt((int)m.MoveType); w.WriteFloat(m.SpeedRate); },
                r => new PlayerSpeedRateChanged((UnitMoveType)r.ReadInt(), r.ReadFloat()));

            Register<PlayerRootChanged>(
                (w, m) => { w.WriteBool(m.Applied); },
                r => new PlayerRootChanged(r.ReadBool()));

            Register<PlayerMovementControlChanged>(
                (w, m) =>
                {
                    w.WriteBool(m.PlayerHasControl); w.WriteVector3(m.LastServerPosition);
                    w.WriteInt((int)m.LastServerMovementFlags);
                },
                r => new PlayerMovementControlChanged(r.ReadBool(), r.ReadVector3(), (MovementFlags)r.ReadInt()));

            Register<UnitChatMessage>(
                (w, m) => { w.WriteNetId(m.SenderId); w.WriteString(m.SenderName); w.WriteString(m.Message); },
                r => new UnitChatMessage(r.ReadNetId(), r.ReadString(), r.ReadString()));

            Register<LoadScenarioCommand>(
                (w, m) => { w.WriteInt(m.ScenarioIndex); },
                r => new LoadScenarioCommand(r.ReadInt()));

            Register<EndScenarioCommand>(
                (w, m) => { },
                r => new EndScenarioCommand());

            Register<RequestScenarioCommand>(
                (w, m) => { },
                r => new RequestScenarioCommand());

            // ---- Entity-scoped ----
            Register<UnitSpellHit>(
                (w, m) => { w.WriteNetId(m.TargetId); w.WriteInt(m.SpellId); },
                r => new UnitSpellHit(r.ReadNetId(), r.ReadInt()));

            Register<UnitSpellDamage>(
                (w, m) => { w.WriteNetId(m.CasterId); w.WriteInt(m.Damage); w.WriteInt((int)m.HitType); },
                r => new UnitSpellDamage(r.ReadNetId(), r.ReadInt(), (HitType)r.ReadInt()));

            Register<UnitSpellLaunch>(
                (w, m) =>
                {
                    w.WriteNetId(m.CasterId);
                    w.WriteInt(m.SpellId);
                    w.WriteVector3(m.Source);
                    w.WriteVector3(m.Destination);
                    int count = m.Targets?.Count ?? 0;
                    w.WriteInt(count);
                    for (int i = 0; i < count; i++)
                    {
                        w.WriteNetId(m.Targets[i].TargetId);
                        w.WriteFloat(m.Targets[i].Time);
                    }
                },
                r =>
                {
                    NetId caster = r.ReadNetId();
                    int spellId = r.ReadInt();
                    var source = r.ReadVector3();
                    var destination = r.ReadVector3();
                    int count = r.ReadInt();
                    var targets = new SpellLaunchTarget[count];
                    for (int i = 0; i < count; i++)
                        targets[i] = new SpellLaunchTarget(r.ReadNetId(), r.ReadFloat());
                    return new UnitSpellLaunch(caster, spellId, source, destination, targets);
                });
        }
    }
}
