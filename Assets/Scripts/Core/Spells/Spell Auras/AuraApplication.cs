namespace Core
{
    public class AuraApplication
    {
        public Unit Target { get; private set; }
        public Aura BaseAura { get; private set; }
        public AuraRemoveMode RemoveMode { get; set; }                                      // Store info for know remove aura reason
        public AuraFlags Flags { get; private set; }                                        // Aura info flag

        public int Slot { get; private set; }                                               // Aura slot on unit
        public bool IsPositive => (Flags & AuraFlags.Positive) != 0;
        public bool IsSelfcast => (Flags & AuraFlags.Nocaster) != 0;
        public int EffectsToApply { get; private set; }                                     // Used only at spell hit to determine which effect should be applied
        public int EffectMask { get; private set; }

        public bool NeedClientUpdate { get; set; }

        public AuraApplication(Unit target, Unit caster, Aura baseAura, uint effMask) { }

        private void Remove() { }
        private void InitFlags(Unit caster, uint effMask) { }
        private void HandleEffect(int effIndex, bool apply) { }

        public bool HasEffect(int effect) { return (EffectMask & (1 << effect)) != 0; }
    
        public void SetNeedClientUpdate() { }
        public void ClientUpdate(bool remove = false) { }
    }
}