using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public sealed class UnitModel : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private TagContainer tagContainer;
        [SerializeField, UsedImplicitly] private SkinnedMeshRenderer meshRenderer;
        [SerializeField, UsedImplicitly] private Animator animator;
        [SerializeField, UsedImplicitly] private float strafeSpeed = 1.0f;

        private Unit Unit => Renderer.Unit;

        public TagContainer TagContainer => tagContainer;
        public Animator Animator => animator;
        public UnitRenderer Renderer { get; private set; }
        public UnitModelSettings Settings { get; private set; }

        private readonly Dictionary<Material, Material> sharedMaterialsByInstancedTransparentMaterials = new Dictionary<Material, Material>();
        private Material[] originalMaterials;
        private Material[] transparentMaterialInstances;

        private bool unitHasTransparency;
        private bool transparentMaterialsUsed;
        private float targetAlpha = 1.0f;
        private float currentAlpha = 1.0f;

        [UsedImplicitly]
        private void OnDestroy() => DestroyTransparentMaterials();

        public void Initialize(UnitRenderer unitRenderer, UnitModelSettings modelSettings)
        {
            if (originalMaterials == null)
                originalMaterials = meshRenderer.sharedMaterials;

            transform.SetParent(unitRenderer.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            Renderer = unitRenderer;
            Settings = modelSettings;

            if (Renderer.Unit.IsDead)
            {
                animator.SetBool("IsDead", true);
                animator.Play("Death");
            }
        }

        public void Deinitialize()
        {
            Animator.WriteDefaultValues();

            currentAlpha = 1.0f;
            targetAlpha = 1.0f;

            Settings = null;
            Renderer = null;

            meshRenderer.materials = originalMaterials;
            unitHasTransparency = false;
        }

        public void DoUpdate(float deltaTime)
        {
            UpdateAnimations(deltaTime);
            UpdateTransparencyTransition(deltaTime);
        }

        public void TriggerInstantCast()
        {
            if (Animator.GetBool(AnimatorUtils.ResurrectingAnimationParam) || Animator.GetBool(AnimatorUtils.DyingAnimationParam))
                return;

            Animator.Play(AnimatorUtils.SpellCastAnimationState, 0, 0.1f);
            Animator.ResetTrigger(AnimatorUtils.SpellCastAnimationTrigger);

            // Switch leg animation for casting
            if (!animator.GetBool("Grounded"))
                animator.Play("Air", 1);
            else if (animator.GetFloat("Speed") > 0.1f)
                animator.Play("Run", 1);
            else
                animator.Play("Cast", 1, 0.1f);
        }

        public void HandleVisualEffects(bool instantly)
        {
            bool isUnitTransperent = Unit.VisualEffects.HasTargetFlag(UnitVisualEffectFlags.StealthTransparency);
            if (isUnitTransperent && !unitHasTransparency)
                ToggleTransparentMode(true, instantly);
            else if (!isUnitTransperent && unitHasTransparency)
                ToggleTransparentMode(false, instantly);
        }

        private void ToggleTransparentMode(bool apply, bool instantly)
        {
            unitHasTransparency = apply;

            targetAlpha = apply ? Renderer.Settings.StealthTransparencyAlpha : 1.0f;

            if (apply && transparentMaterialInstances == null)
                CreateTransparentMaterials();

            if (instantly)
                currentAlpha = targetAlpha;

            if (apply || instantly)
                SwitchTransparentMaterials(apply);
        }

        private void UpdateTransparencyTransition(float deltaTime)
        {
            if (transparentMaterialsUsed && !Mathf.Approximately(currentAlpha, targetAlpha))
            {
                currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, deltaTime * Renderer.Settings.TransparencyTransitionSpeed);

                if (currentAlpha >= 1.0f)
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
                newColor.a = currentAlpha;
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

        private void UpdateAnimations(float deltaTime)
        {
            if (!Renderer.Unit.IsAlive)
            {
                animator.SetBool("IsDead", true);
                return;
            }

            if (!Renderer.Unit.HasMovementFlag(MovementFlags.Flying))
            {
                Animator.SetBool("Grounded", true);

                float currentStrafe = Animator.GetFloat("Strafe");
                float strafeTarget = Renderer.Unit.HasMovementFlag(MovementFlags.StrafeLeft) ? 0 :
                    Renderer.Unit.HasMovementFlag(MovementFlags.StrafeRight) ? 1 : 0.5f;

                float strafeDelta = 2 * Mathf.Sign(strafeTarget - currentStrafe) * deltaTime * strafeSpeed;
                float resultStrafe = Mathf.Clamp(currentStrafe + strafeDelta, 0.0f, 1.0f);

                if (Mathf.Abs(strafeTarget - currentStrafe) > Mathf.Abs(strafeDelta))
                    Animator.SetFloat("Strafe", resultStrafe);

                if (Renderer.Unit.HasMovementFlag(MovementFlags.Forward))
                    Animator.SetFloat("Forward", 1.0f);
                else
                    Animator.SetFloat("Forward", Mathf.Clamp(Animator.GetFloat("Forward") - 10 * deltaTime, 0.0f, 1.0f));

                if (Renderer.Unit.HasMovementFlag(MovementFlags.Forward | MovementFlags.Backward | MovementFlags.StrafeRight | MovementFlags.StrafeLeft))
                    Animator.SetFloat("Speed", 1);
                else
                    Animator.SetFloat("Speed", Mathf.Clamp(Animator.GetFloat("Speed") - 10 * deltaTime, 0.0f, 1.0f));
            }
            else
                Animator.SetBool("Grounded", false);
        }
    }
}