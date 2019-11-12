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
                private Vector3 lastKnownTargetPosition;

                private int Delay { get; }
                private int ServerLaunchFrame { get; }
                private int ExpectedDelayFrames { get; }

                private EffectSpellSettings Settings { get; set; }
                private UnitRenderer TargetRenderer { get; set; }

                private IEffectEntity projectile;
                private long playId;

                public SpellVisualProjectile(UnitRenderer target, EffectSpellSettings settings, int serverLaunchFrame, int delay)
                {
                    Delay = delay;
                    ServerLaunchFrame = serverLaunchFrame;
                    TargetRenderer = target;
                    Settings = settings;

                    ExpectedDelayFrames = (int)(Delay / BoltNetwork.FrameDeltaTime / 1000.0f);
                }

                public bool HandleLaunch(UnitRenderer caster)
                {
                    Vector3 forward = TargetRenderer.transform.position - caster.transform.position;
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
                        lastKnownTargetPosition = TargetRenderer.TagContainer.FindTag(DefaultTargetTag);
                        TargetRenderer = null;
                    }
                }

                public bool DoUpdate()
                {
                    if (!projectile.IsPlaying(playId))
                        return true;

                    if (TargetRenderer != null)
                        lastKnownTargetPosition = TargetRenderer.TagContainer.FindTag(DefaultTargetTag);

                    float ratio = (float)(BoltNetwork.ServerFrame - ServerLaunchFrame) / ExpectedDelayFrames;
                    projectile.Transform.position = Vector3.Lerp(casterLaunchPosition, lastKnownTargetPosition, ratio);

                    if (lastKnownTargetPosition != projectile.Transform.position)
                        projectile.Transform.rotation = Quaternion.LookRotation(lastKnownTargetPosition - projectile.Transform.position);

                    return ratio >= 1.0f;
                }
            }
        }
    }
}