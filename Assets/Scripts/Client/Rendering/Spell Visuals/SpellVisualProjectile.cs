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
                private Vector3 casterLaunchPosition;

                private int Delay { get; }
                private int ServerLaunchFrame { get; }
                private int ExpectedDelayFrames { get; }

                private EffectSpellSettings Settings { get; set; }
                private UnitRenderer TargetRenderer { get; set; }
                private Vector3? Destination { get; set; }
                private IEffectEntity projectile;
                private long playId;

                public SpellVisualProjectile(Vector3 destination, EffectSpellSettings settings, int serverLaunchFrame, int delay)
                    : this(settings, serverLaunchFrame, delay)
                {
                    TargetRenderer = null;
                    Destination = destination;
                }

                public SpellVisualProjectile(UnitRenderer target, EffectSpellSettings settings, int serverLaunchFrame, int delay)
                    :this(settings, serverLaunchFrame, delay)
                {
                    TargetRenderer = target;
                    Destination = null;
                }

                private SpellVisualProjectile(EffectSpellSettings settings, int serverLaunchFrame, int delay)
                {
                    Delay = delay;
                    ServerLaunchFrame = serverLaunchFrame;
                    Settings = settings;

                    ExpectedDelayFrames = (int)(Delay / BoltNetwork.FrameDeltaTime / 1000.0f);
                }

                public bool HandleLaunch(UnitRenderer caster)
                {
                    UpdateDestination();

                    if (!Destination.HasValue)
                        return false;

                    Vector3 forward = Destination.Value - caster.transform.position;
                    projectile = Settings.EffectSettings.PlayEffect(Vector3.zero, Quaternion.LookRotation(forward), out long newPlayId);

                    if (projectile != null)
                    {
                        casterLaunchPosition = caster.TagContainer.FindDefaultLaunchTag();
                        caster.TagContainer.ApplyPositioning(projectile, Settings);
                        playId = newPlayId;
                        return true;
                    }

                    return false;
                }

                public void HandleFinish(bool instant)
                {
                    if(instant)
                        projectile?.Stop(playId);
                    else
                        projectile?.Fade(playId);

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

                public bool DoUpdate()
                {
                    if (!projectile.IsPlaying(playId))
                        return true;

                    UpdateDestination();

                    float ratio = (float)(BoltNetwork.ServerFrame - ServerLaunchFrame) / ExpectedDelayFrames;
                    if (Destination.HasValue)
                    {
                        projectile.Transform.position = Vector3.Lerp(casterLaunchPosition, Destination.Value, ratio);

                        if (Destination != projectile.Transform.position)
                            projectile.Transform.rotation = Quaternion.LookRotation(Destination.Value - projectile.Transform.position);
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