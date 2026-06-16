namespace Net
{
    /// <summary>
    /// Audience filter for an entity-scoped message, relative to that entity's owner/controller.
    /// Replaces Bolt's EntityTargets (Everyone / EveryoneExceptOwner / EveryoneExceptController …).
    /// </summary>
    public enum EntityScope
    {
        /// <summary>All peers observing the entity (EntityTargets.Everyone).</summary>
        All,

        /// <summary>All observers except the entity's owner (EntityTargets.EveryoneExceptOwner).</summary>
        ExceptOwner,

        /// <summary>All observers except the entity's controller (EntityTargets.EveryoneExceptController).</summary>
        ExceptController,

        /// <summary>Only the entity's owner.</summary>
        OwnerOnly,

        /// <summary>Only the entity's controller.</summary>
        ControllerOnly,
    }
}
