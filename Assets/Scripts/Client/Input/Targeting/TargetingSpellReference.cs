using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// Handles client-side target selection for destination spells.
    /// </summary>
    [CreateAssetMenu(fileName = "Targeting Spell Reference", menuName = "Game Data/Scriptable/Spell Targeting", order = 11)]
    public class TargetingSpellReference : ScriptableReferenceClient
    {
        [SerializeField, UsedImplicitly] private InputReference input;
        [SerializeField, UsedImplicitly] private CameraReference cameraReference;
        [SerializeField, UsedImplicitly] private Projector selectionCirclePrototype;
        [SerializeField, UsedImplicitly] private Color validColor;
        [SerializeField, UsedImplicitly] private Color invalidColor;

        private Projector selectionCircle;
        private SpellInfo trackingSpellInfo;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            GameObjectPool.PreInstantiate(selectionCirclePrototype, 1);
        }

        protected override void OnUnregister()
        {
            selectionCircle = null;
            trackingSpellInfo = null;

            base.OnUnregister();
        }

        protected override void OnPlayerControlGained(Player player)
        {
            base.OnPlayerControlGained(player);

            selectionCircle = GameObjectPool.Take(selectionCirclePrototype);
        }

        protected override void OnPlayerControlLost(Player player)
        {
            base.OnPlayerControlLost(player);

            CancelTracking();

            GameObjectPool.Return(selectionCircle, false);

            selectionCircle = null;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (trackingSpellInfo == null)
                return;

            if (Input.GetMouseButton(1))
                CancelTracking();
            else if (Input.GetMouseButton(0))
            {
                input.CastSpellWithDestination(trackingSpellInfo.Id, selectionCircle.transform.position);

                CancelTracking();
            }
            else
                UpdateCircle();
        }

        private void UpdateCircle()
        {
            Ray ray = cameraReference.WarcraftCamera.Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, float.MaxValue, PhysicsReference.Mask.Ground))
            {
                selectionCircle.enabled = true;
                selectionCircle.transform.position = hit.point;
                selectionCircle.material.color = Vector3.Distance(Player.Position, hit.point) < trackingSpellInfo.GetMaxRange(false) ? validColor : invalidColor;
            }
            else
                selectionCircle.enabled = false;
        }

        private void CancelTracking()
        {
            trackingSpellInfo = null;
            selectionCircle.enabled = false;
        }

        public void SelectSpellDestination(SpellInfo spellInfo)
        {
            Assert.AreEqual(spellInfo.ExplicitTargetType, SpellExplicitTargetType.Destination);

            selectionCircle.orthographicSize = spellInfo.MaxTargetingRadius;
            trackingSpellInfo = spellInfo;
            UpdateCircle();
        }
    }
}
