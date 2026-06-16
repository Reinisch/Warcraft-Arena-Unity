using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Client
{
    public class TargetingSpellReference : ScriptableReferenceClient
    {
        [Inject] private InputReference input;
        [Inject] private CameraReference cameraReference;
        [SerializeField, UsedImplicitly] private SpellInfo shootingSpellInfo;
        [SerializeField, UsedImplicitly] private DecalProjector selectionCirclePrototype;
        [SerializeField, UsedImplicitly] private Color validColor;
        [SerializeField, UsedImplicitly] private Color invalidColor;

        private DecalProjector selectionCircle;
        private Material selectionCircleMaterial;
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
            selectionCircleMaterial = null;
            targetingSpellInfo = null;

            base.OnUnregister();
        }

        public override void OnControlStateChanged(bool underControl)
        {
            if (underControl)
            {
                base.OnControlStateChanged(true);

                selectionCircle = GameObjectPool.Take(selectionCirclePrototype);
                selectionCircleMaterial = selectionCircle.material;
                StopTargeting();
            }
            else
            {
                StopTargeting();
                GameObjectPool.Return(selectionCircle, false);
                selectionCircle = null;
                selectionCircleMaterial = null;

                base.OnControlStateChanged(false);
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            bool usingDestination = HandleDestinationSpells();

            if (!usingDestination)
            {
                HandleShootingSpells();
            }

            bool HandleDestinationSpells()
            {
                if (targetingSpellInfo == null)
                    return false;

                if (input.RightClickPressed)
                    StopTargeting();
                else if (input.LeftClickPressed)
                {
                    input.CastSpellWithDestination(targetingSpellInfo.Id, selectionCircle.transform.position);

                    StopTargeting();
                }
                else
                    UpdateCircle();

                return true;
            }

            void HandleShootingSpells()
            {
                if (shootingSpellInfo == null || Player is not { MovementMode: MovementMode.Shooter } || input.IsAlternativeMode)
                    return;

                if (input.LeftClickPressed)
                {
                    input.CastSpellWithTargetingOptions(shootingSpellInfo.Id);
                }
            }
        }

        private void UpdateCircle()
        {
            Ray ray = cameraReference.WarcraftCamera.Camera.ScreenPointToRay(input.MousePosition);
            if (Physics.Raycast(ray, out var hit, float.MaxValue, PhysicsReference.Mask.Ground))
            {
                selectionCircle.enabled = true;
                selectionCircle.transform.position = hit.point;
                selectionCircleMaterial.color = Vector3.Distance(Player.Position, hit.point) < targetingSpellInfo.GetMaxRange(false) ? validColor : invalidColor;
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

            selectionCircle.size = new Vector3(spellInfo.MaxTargetingRadius * 2, selectionCircle.size.y, spellInfo.MaxTargetingRadius * 2);
            targetingSpellInfo = spellInfo;
            UpdateCircle();
        }
    }
}
