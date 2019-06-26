using Client.Spells;
using UnityEngine;

namespace Client
{
    public partial class SpellVisualController
    {
        private class SpellVisualEntry
        {
            public int Delay { get; private set; }
            public int ServerLaunchFrame { get; private set; }
            public int ExpectedDelayFrames { get; private set; }

            public UnitRenderer CasterRenderer { get; private set; }
            public UnitRenderer TargetRenderer { get; private set; }
            public EffectSpellSettings Settings { get; private set; }

            private IEffectEntity projectile;
            private long playId;

            public SpellVisualEntry(UnitRenderer caster, UnitRenderer target, EffectSpellSettings settings, int serverLaunchFrame, int delay)
            {
                Delay = delay;
                ServerLaunchFrame = serverLaunchFrame;
                CasterRenderer = caster;
                TargetRenderer = target;
                Settings = settings;

                ExpectedDelayFrames = (int) (Delay / BoltNetwork.FrameDeltaTime / 1000.0f);
            }

            public bool HandleLaunch()
            {
                Vector3 forward = TargetRenderer.transform.position - CasterRenderer.transform.position;
                projectile = Settings.EffectSettings.PlayEffect(Vector3.zero, Quaternion.LookRotation(forward), out long newPlayId);
                CasterRenderer.TagContainer.ApplyPositioning(projectile, Settings);
                playId = newPlayId;

                return projectile != null;
            }

            public void HandleFinish()
            {
                projectile?.Stop(playId);

                CasterRenderer = null;
                TargetRenderer = null;
                Settings = null;
            }

            public bool DoUpdate(float deltaTime)
            {
                if (!projectile.IsPlaying(playId))
                    return true;

                float ratio = (float)(BoltNetwork.ServerFrame - ServerLaunchFrame) / ExpectedDelayFrames;
                Vector3 sourceTag = CasterRenderer.TagContainer.FindDefaultLaunchTag();
                Vector3 targetTag = TargetRenderer.TagContainer.FindTag(DefaultTargetTag);

                projectile.Transform.position = Vector3.Lerp(sourceTag, targetTag, ratio);
                projectile.Transform.rotation = Quaternion.LookRotation(targetTag - sourceTag);

                return ratio >= 1.0f;
            }
        }
    }
}