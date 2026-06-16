using Client.Spells;
using UnityEngine;

namespace Client
{
    public partial class RenderingReference
    {
        private partial class SpellVisualController
        {
            private class SpellVisualProjectile
            {
                private Vector3 launchSource;

                private float Duration { get; }
                private float DurationLeft { get; set; }

                private EffectSpellSettings Settings { get; set; }
                private UnitRenderer TargetRenderer { get; set; }
                private Vector3? Destination { get; set; }
                private EffectHandle handle;
                private bool explicitSource;

                public SpellVisualProjectile(Vector3 source, Vector3 destination, EffectSpellSettings settings, float duration, bool sourceIsExplicit)
                    : this(settings, source, duration, sourceIsExplicit)
                {
                    TargetRenderer = null;
                    Destination = destination;
                }

                public SpellVisualProjectile(Vector3 source, UnitRenderer target, EffectSpellSettings settings, float duration, bool sourceIsExplicit)
                    : this(settings, source, duration, sourceIsExplicit)
                {
                    TargetRenderer = target;
                    Destination = null;
                }

                private SpellVisualProjectile(EffectSpellSettings settings, Vector3 source, float duration, bool sourceIsExplicit)
                {
                    Duration = DurationLeft = duration;
                    Settings = settings;

                    launchSource = source;
                    explicitSource = sourceIsExplicit;
                }

                public bool HandleLaunch(UnitRenderer caster)
                {
                    UpdateDestination();

                    if (!Destination.HasValue)
                        return false;

                    Vector3 forward = Destination.Value - caster.transform.position;
                    handle = Settings.EffectSettings.PlayEffect(Vector3.zero, Quaternion.LookRotation(forward));

                    if (handle.IsValid)
                    {
                        caster.TagContainer.ApplyPositioning(handle.Entity, Settings);

                        if (!explicitSource)
                            launchSource = caster.TagContainer.FindDefaultLaunchTag();

                        handle.Entity.Transform.position = launchSource;

                        return true;
                    }

                    return false;
                }

                public void HandleFinish(bool instant)
                {
                    if(instant)
                        handle.Stop();
                    else
                        handle.Fade();

                    TargetRenderer = null;
                    Settings = null;
                }

                public void HandleRendererDetach(UnitRenderer targetRenderer)
                {
                    if (TargetRenderer == targetRenderer)
                    {
                        UpdateDestination();
                        TargetRenderer = null;
                    }
                }

                public bool DoUpdate(float deltaTime)
                {
                    if (!handle.IsValid)
                        return true;

                    UpdateDestination();

                    DurationLeft = Mathf.MoveTowards(DurationLeft, 0.0f, deltaTime);
                    float ratio = 1 - DurationLeft / Duration;
                    if (Destination.HasValue)
                    {
                        handle.Entity.Transform.position = Vector3.Lerp(launchSource, Destination.Value, ratio);

                        if (Destination != handle.Entity.Transform.position)
                            handle.Entity.Transform.rotation = Quaternion.LookRotation(Destination.Value - handle.Entity.Transform.position);
                    }

                    return ratio >= 1.0f;
                }

                private void UpdateDestination()
                {
                    if (TargetRenderer != null)
                        Destination = TargetRenderer.TagContainer.FindTag(DefaultTargetTag);
                }
            }
        }
    }
}