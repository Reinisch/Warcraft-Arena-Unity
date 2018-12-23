using UnityEngine;

namespace Core
{
    public class CreatureAI : UnitAI
    {
        protected new Creature Me { get => base.Me as Creature; set => base.Me = value; }

        private bool MoveInLineOfSightLocked { get; set; }

        public CreatureAI(Creature creature) : base(creature)
        {
            MoveInLineOfSightLocked = false;
        }

        public override void UpdateAI(uint diff)
        {
        }

        public void Talk(byte id, WorldEntity whisperTarget = null)
        {
        }

        #region Reactions on events

        // Called if IsVisible(Unit* who) is true at each who move, reaction at visibility zone enter
        public void MoveInLineOfSight_Safe(Unit who)
        {
        }

        // Trigger Creature "Alert" state (creature can see stealthed unit)
        public void TriggerAlert(Unit who)
        {
        }

        // Called in Creature::Update when deathstate = DEAD. Inherited classes may maniuplate the ability to respawn based on scripted events.
        public virtual bool CanRespawn()
        {
            return true;
        }

        // Called for reaction at stopping attack at no attackers or targets
        public virtual void EnterEvadeMode(EvadeReason why = EvadeReason.Other)
        {
        }

        // Called for reaction at enter to combat if not in combat yet (enemy can be NULL)
        public virtual void EnterCombat(Unit victim)
        {
        }

        // Called when the creature is killed
        public virtual void JustDied(Unit killer)
        {
        }

        // Called when the creature kills a unit
        public virtual void KilledUnit(Unit victim)
        {
        }

        // Called when the creature summon successfully other creature
        public virtual void JustSummoned(Creature summon)
        {
        }

        public virtual void IsSummonedBy(Unit summoner)
        {
        }

        public virtual void SummonedCreatureDespawn(Creature summon)
        {
        }

        public virtual void SummonedCreatureDies(Creature summon, Unit killer)
        {
        }

        // Called when the creature successfully summons a gameobject
        public virtual void JustSummonedGameobject(WorldEntity entity)
        {
        }

        public virtual void SummonedGameobjectDespawn(WorldEntity entity)
        {
        }

        // Called when the creature successfully registers a dynamicobject
        public virtual void JustRegisteredDynObject(DynamicEntity dynEntity)
        {
        }

        public virtual void JustUnregisteredDynObject(DynamicEntity dynEntity)
        {
        }

        // Called when hit by a spell
        public virtual void SpellHit(Unit caster, SpellInfo spell)
        {
        }

        // Called when spell hits a target
        public virtual void SpellHitTarget(Unit target, SpellInfo spell)
        {
        }

        // Called when the creature is target of hostile action: swing, hostile spell landed, fear/etc)
        public virtual void AttackedBy(Unit attacker)
        {
        }

        public virtual bool IsEscorted()
        {
            return false;
        }

        // Called when creature is spawned or respawned
        public virtual void JustRespawned()
        {
        }

        // Called at waypoint reached or point movement finished
        public virtual void MovementInform(uint type, uint id)
        {
        }

        public override void OnCharmed(bool apply)
        {
        }

        // Called at reaching home after evade
        public virtual void JustReachedHome()
        {
        }

        public void DoZoneInCombat(Creature creature = null, float maxRangeToNearestTarget = 50.0f)
        {
        }

        // Called at text emote receive from player
        public virtual void ReceiveEmote(Player player, uint emoteId)
        {
        }

        // Called when owner takes damage
        public virtual void OwnerAttackedBy(Unit attacker)
        {
        }

        // Called when owner attacks something
        public virtual void OwnerAttacked(Unit target)
        {
        }

        #endregion

        #region State checks

        // Is unit visible for MoveInLineOfSight
        public virtual bool IsVisible(Unit unit)
        {
            return false;
        }

        // called when the corpse of this creature gets removed
        public virtual void CorpseRemoved(ref uint respawnDelay)
        {
        }

        // Called when victim entered water and creature can not enter water
        public virtual bool CanReachByRangeAttack(Unit unit)
        {
            return false;
        }

        #endregion

        public virtual void PassengerBoarded(Unit passenger, byte seatId, bool apply)
        {
        }

        public virtual void OnSpellClick(Unit clicker, ref bool result)
        {
        }

        public virtual bool CanSeeAlways(WorldEntity entity)
        {
            return false;
        }

        public virtual UnitAI GetAIForCharmedPlayer(Player who)
        {
            return null;
        }

        public virtual bool CheckInRoom()
        {
            return false;
        }

        protected bool UpdateVictim()
        {
            return false;
        }

        protected bool UpdateVictimWithGaze()
        {
            return false;
        }

        protected void SetGazeOn(Unit target)
        {
        }

        protected virtual void MoveInLineOfSight(Unit who)
        {
        }

        protected Creature DoSummon(uint entry, Vector3 pos, uint despawnTime = 30000,
            TempSummonType summonType = TempSummonType.TimedDespawn)
        {
            return null;
        }

        protected Creature DoSummon(uint entry, WorldEntity obj, float radius = 5.0f, uint despawnTime = 30000,
            TempSummonType summonType = TempSummonType.TimedDespawn)
        {
            return null;
        }

        protected Creature DoSummonFlyer(uint entry, WorldEntity obj, float flightZ, float radius = 5.0f,
            uint despawnTime = 30000, TempSummonType summonType = TempSummonType.TimedDespawn)
        {
            return null;
        }
    }
}