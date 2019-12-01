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
        private SpellInfo targetingSpellInfo;

        public bool IsTargeting => targetingSpellInfo != null;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            GameObjectPool.PreInstantiate(selectionCirclePrototype, 1);
        }

        protected override void OnUnregister()
        {
            selectionCircle = null;
            targetingSpellInfo = null;

            base.OnUnregister();
        }

        protected override void OnControlStateChanged(Player player, bool underControl)
        {
            if (underControl)
            {
                base.OnControlStateChanged(player, true);

                selectionCircle = GameObjectPool.Take(selectionCirclePrototype);
                StopTargeting();
            }
            else
            {
                StopTargeting();
                GameObjectPool.Return(selectionCircle, false);
                selectionCircle = null;

                base.OnControlStateChanged(player, false);
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (targetingSpellInfo == null)
                return;

            if (Input.GetMouseButton(1))
                StopTargeting();
            else if (Input.GetMouseButton(0))
            {
                input.CastSpellWithDestination(targetingSpellInfo.Id, selectionCircle.transform.position);

                StopTargeting();
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
                selectionCircle.material.color = Vector3.Distance(Player.Position, hit.point) < targetingSpellInfo.GetMaxRange(false) ? validColor : invalidColor;
            }
            else
                selectionCircle.enabled = false;
        }

        public void StopTargeting()
        {
            targetingSpellInfo = null;
            selectionCircle.enabled = false;
        }

        public void SelectSpellTargetDestination(SpellInfo spellInfo)
        {
            Assert.AreEqual(spellInfo.ExplicitTargetType, SpellExplicitTargetType.Destination);

            selectionCircle.orthographicSize = spellInfo.MaxTargetingRadius;
            targetingSpellInfo = spellInfo;
            UpdateCircle();
        }
    }
}
