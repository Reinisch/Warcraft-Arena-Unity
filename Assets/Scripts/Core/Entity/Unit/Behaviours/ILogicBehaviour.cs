namespace Core
{
    /// <summary>
    /// Opt-in marker for unit behaviours that only run under a specific networking role.
    /// A behaviour is added by the <see cref="Unit.BehaviourController"/> only if its declared logic
    /// intersects the <see cref="World"/>'s active logic (see World.HasServerLogic / HasClientLogic).
    /// Behaviours that do NOT implement this run everywhere (e.g. local movement / motion), which is
    /// the single-player default. This is what keeps a remote client from running server-authoritative
    /// behaviours (combat, aura application, AI) — it just renders replicated state and events.
    /// </summary>
    internal interface ILogicBehaviour
    {
        bool HasServerLogic { get; }
        bool HasClientLogic { get; }
    }
}
