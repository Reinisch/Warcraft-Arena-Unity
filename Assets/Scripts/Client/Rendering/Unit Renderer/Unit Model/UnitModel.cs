using System;
using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public sealed class UnitModel : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private RenderingReference rendering;
        [SerializeField, UsedImplicitly] private TagContainer tagContainer;
        [SerializeField, UsedImplicitly] private SkinnedMeshRenderer meshRenderer;
        [SerializeField, UsedImplicitly] private Animator animator;
        [SerializeField, UsedImplicitly] private float strafeSpeed = 1.0f;
        [SerializeField, UsedImplicitly] private List<Collider> hitBoxes;

        public IReadOnlyList<Collider> HitBoxes => hitBoxes;
        public TagContainer TagContainer => tagContainer;
        public Animator Animator => animator;
        public float TargetAlpha { get; private set; } = 1.0f;
        public float CurrentAlpha { get; private set; } = 1.0f;
        public UnitModelSettings Settings { get; private set; }

        private readonly Dictionary<Material, Material> sharedMaterialsByInstancedTransparentMaterials = new Dictionary<Material, Material>();
        private Material[] originalMaterials;
        private Material[] transparentMaterialInstances;

        private bool unitHasTransparency;
        private bool transparentMaterialsUsed;

        [UsedImplicitly]
        private void Awake() => originalMaterials = meshRenderer.sharedMaterials;

        [UsedImplicitly]
        private void OnDestroy() => DestroyTransparentMaterials();

        public void Initialize(UnitModelInitializer initializer)
        {
            transform.SetParent(initializer.UnitRenderer.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            Settings = initializer.ModelSettings;
            Animator.enabled = true;

            if (initializer.UnitRenderer.Unit.IsDead)
            {
                animator.SetBool("IsDead", true);
                animator.Play("Death");
            }

            switch (initializer.ReplacementMode)
            {
                case UnitModelReplacementMode.ScopeIn:
                    ToggleTransparentMode(true, 0.0f, 1.0f);
                    break;
                case UnitModelReplacementMode.Transformation:
                    if (initializer.PreviousModel != null && initializer.PreviousModel.CurrentAlpha < 1.0f)
                        ToggleTransparentMode(true, initializer.PreviousModel.CurrentAlpha, initializer.PreviousModel.TargetAlpha);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(initializer.ReplacementMode), initializer.ReplacementMode, "Unhandled model replacement mode!");
            }

            HandleVisualEffects(initializer.UnitRenderer, false, true);
        }

        public void Deinitialize()
        {
            Animator.WriteDefaultValues();
            Animator.enabled = false;

            CurrentAlpha = 1.0f;
            TargetAlpha = 1.0f;

            Settings = null;

            meshRenderer.materials = originalMaterials;
            unitHasTransparency = false;

            GameObjectPool.Return(this, false);
        }

        public void DoUpdate(UnitRenderer unitRenderer, float deltaTime)
        {
            if (unitRenderer != null)
                UpdateAnimations(unitRenderer, deltaTime);

            UpdateTransparencyTransition(deltaTime);
        }

        public void TriggerInstantCast(SpellInfo spellInfo)
        {
            if (Animator.GetBool(AnimatorUtils.ResurrectingAnimationParam) || Animator.GetBool(AnimatorUtils.DyingAnimationParam))
                return;

            AnimationInfo animationInfo = rendering.FindAnimation(spellInfo);
            int animationHash = Animator.HasState(0, animationInfo.StateHash)
                ? animationInfo.StateHash
                : animationInfo.FallbackStateHash;

            Animator.Play(animationHash, 0, 0.1f);

            if (Animator.layerCount > 1)
            {
                // Switch leg animation for casting
                if (!animator.GetBool("Grounded"))
                    animator.Play("Air", 1);
                else if (animator.GetFloat("Speed") > 0.1f)
                    animator.Play("Run", 1);
                else
                    animator.Play(animationHash, 1, 0.1f);
            }
        }

        public void HandleVisualEffects(UnitRenderer unitRenderer, bool instantly, bool forced = false)
        {
            bool isUnitTransperent = unitRenderer.Unit.VisualEffects.HasAnyFlag(UnitVisualEffectFlags.AnyTransparency);
            if (isUnitTransperent && (!unitHasTransparency || forced))
            {
                float targetAlpha = rendering.UnitRendererSettings.StealthTransparencyAlpha;
                float currentAlpha = instantly ? targetAlpha : CurrentAlpha;

                ToggleTransparentMode(true, currentAlpha, targetAlpha);
            }
            else if (!isUnitTransperent && (unitHasTransparency || forced))
            {
                float targetAlpha = 1.0f;
                float currentAlpha = instantly ? targetAlpha : CurrentAlpha;

                ToggleTransparentMode(false, currentAlpha, targetAlpha);
            }
        }

        public void ToggleTransparentMode(bool apply, float currentAlpha, float targetAlpha)
        {
            unitHasTransparency = apply;

            CurrentAlpha = currentAlpha;
            TargetAlpha = targetAlpha;

            if (apply && transparentMaterialInstances == null)
                CreateTransparentMaterials();

            if (apply || Mathf.Approximately(CurrentAlpha, TargetAlpha))
                SwitchTransparentMaterials(apply);
        }

        private void UpdateTransparencyTransition(float deltaTime)
        {
            if (transparentMaterialsUsed && !Mathf.Approximately(CurrentAlpha, TargetAlpha))
            {
                CurrentAlpha = Mathf.MoveTowards(CurrentAlpha, TargetAlpha, deltaTime * rendering.UnitRendererSettings.TransparencyTransitionSpeed);

                if (CurrentAlpha >= 1.0f)
                    SwitchTransparentMaterials(false);
                else
                    UpdateTransparentColor();
            }
        }

        private void UpdateTransparentColor()
        {
            foreach (var materialEntry in sharedMaterialsByInstancedTransparentMaterials)
            {
                Color newColor = materialEntry.Value.color;
                newColor.a = CurrentAlpha;
                materialEntry.Value.SetColor("_Color", newColor);
            }
        }

        private void SwitchTransparentMaterials(bool useTransparent)
        {
            meshRenderer.materials = useTransparent ? transparentMaterialInstances : originalMaterials;
            transparentMaterialsUsed = useTransparent;

            if (transparentMaterialsUsed)
                UpdateTransparentColor();
        }

        private void CreateTransparentMaterials()
        {
            transparentMaterialInstances = new Material[meshRenderer.sharedMaterials.Length];
            for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
            {
                Material sharedMaterial = meshRenderer.sharedMaterials[i];

                if (!sharedMaterialsByInstancedTransparentMaterials.TryGetValue(sharedMaterial, out Material instancedMaterial))
                {
                    instancedMaterial = new Material(sharedMaterial);
                    instancedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    instancedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    instancedMaterial.SetInt("_ZWrite", 1);
                    instancedMaterial.DisableKeyword("_ALPHATEST_ON");
                    instancedMaterial.DisableKeyword("_ALPHABLEND_ON");
                    instancedMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    instancedMaterial.renderQueue = 3000;

                    sharedMaterialsByInstancedTransparentMaterials.Add(sharedMaterial, instancedMaterial);
                }

                transparentMaterialInstances[i] = instancedMaterial;
            }
        }

        private void DestroyTransparentMaterials()
        {
            if (sharedMaterialsByInstancedTransparentMaterials.Count > 0)
            {
                meshRenderer.materials = originalMaterials;

                foreach (var materialInstanceEntry in sharedMaterialsByInstancedTransparentMaterials)
                    Destroy(materialInstanceEntry.Value);

                sharedMaterialsByInstancedTransparentMaterials.Clear();
                transparentMaterialInstances = null;
            }
        }

        private void UpdateAnimations(UnitRenderer unitRenderer, float deltaTime)
        {
            if (!unitRenderer.Unit.IsAlive)
            {
                animator.SetBool("IsDead", true);
                return;
            }

            bool isFlying = unitRenderer.Unit.HasMovementFlag(MovementFlags.Flying);
            bool isCharging = unitRenderer.Unit.HasMovementFlag(MovementFlags.Charging);

            if (!isFlying || isCharging)
            {
                Animator.SetBool("Grounded", !isFlying);

                float currentStrafe = Animator.GetFloat("Strafe");
                float strafeTarget = unitRenderer.Unit.HasMovementFlag(MovementFlags.StrafeLeft) ? 0 :
                    unitRenderer.Unit.HasMovementFlag(MovementFlags.StrafeRight) ? 1 : 0.5f;

                float strafeDelta = 2 * Mathf.Sign(strafeTarget - currentStrafe) * deltaTime * strafeSpeed;
                float resultStrafe = Mathf.Clamp(currentStrafe + strafeDelta, 0.0f, 1.0f);

                if (Mathf.Abs(strafeTarget - currentStrafe) > Mathf.Abs(strafeDelta))
                    Animator.SetFloat("Strafe", resultStrafe);

                if (unitRenderer.Unit.HasMovementFlag(MovementFlags.Forward))
                    Animator.SetFloat("Forward", Mathf.MoveTowards(Animator.GetFloat("Forward"), 1.0f, 10 * deltaTime));
                else if (unitRenderer.Unit.HasMovementFlag(MovementFlags.Backward))
                    Animator.SetFloat("Forward", Mathf.MoveTowards(Animator.GetFloat("Forward"), -1.0f, 10 * deltaTime));
                else
                    Animator.SetFloat("Forward", Mathf.MoveTowards(Animator.GetFloat("Forward"), 0.0f, 10 * deltaTime));

                if (unitRenderer.Unit.HasMovementFlag(MovementFlags.Forward | MovementFlags.Backward | MovementFlags.StrafeRight | MovementFlags.StrafeLeft))
                    Animator.SetFloat("Speed", 1);
                else
                    Animator.SetFloat("Speed", Mathf.Clamp(Animator.GetFloat("Speed") - 10 * deltaTime, 0.0f, 1.0f));
            }
            else
                Animator.SetBool("Grounded", false);
        }
    }
}