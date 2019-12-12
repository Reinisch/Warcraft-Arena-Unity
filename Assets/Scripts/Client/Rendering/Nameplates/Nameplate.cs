using System;
using Common;
using Core;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Client
{
    public class Nameplate : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private CanvasGroup combinedCanvasGroup;
        [SerializeField, UsedImplicitly] private CanvasGroup generalCanvasGroup;
        [SerializeField, UsedImplicitly] private HealthFrame healthFrame;
        [SerializeField, UsedImplicitly] private GameObject contentFrame;
        [SerializeField, UsedImplicitly] private CastFrame castFrame;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI unitName;
        [SerializeField, UsedImplicitly] private CameraReference cameraReference;
        [SerializeField, UsedImplicitly] private RenderingReference renderReference;
        [SerializeField, UsedImplicitly] private InterfaceReference interfaceReference;
        [SerializeField, UsedImplicitly] private NameplateSettings nameplateSettings;
        [SerializeField, UsedImplicitly] private GameOptionBool showDeselectedHealthOption;

        private readonly Action onFactionChangedAction;

        private bool InDetailRange { get; set; }
        private NameplateSettings.HostilitySettings HostilitySettings { get; set; }

        public UnitRenderer UnitRenderer { get; private set; }

        private Nameplate()
        {
            onFactionChangedAction = OnFactionChanged;
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            GameObjectPool.Return(this, true);
        }

        public void UpdateUnit(UnitRenderer unitRenderer)
        {
            if (UnitRenderer != null)
                Deinitialize();

            if (unitRenderer != null)
                Initialize(unitRenderer);

            combinedCanvasGroup.alpha = UnitRenderer != null ? 1.0f : 0.0f;
        }

        public void UpdateSelection(bool instantAlphaTransition = false)
        {
            Player referer = renderReference.Player;
            Unit target = UnitRenderer.Unit;

            bool isSelected = referer.Target == target;
            bool showDetails = InDetailRange || isSelected;
            bool showHealth = showDetails && HostilitySettings.ShowHealth && (isSelected || showDeselectedHealthOption.Value);

            generalCanvasGroup.alpha = isSelected ? HostilitySettings.SelectedGeneralAlpha : HostilitySettings.DeselectedGeneralAlpha;
            castFrame.gameObject.SetActive(showDetails && HostilitySettings.ShowCast);
            unitName.color = showHealth ? HostilitySettings.NameWithPlateColor : HostilitySettings.NameWithoutPlateColor;

            healthFrame.TargetFrameAlpha = showHealth ? 1.0f : 0.0f;
            if (instantAlphaTransition || isSelected && showHealth)
                healthFrame.CurrentFrameAlpha = healthFrame.TargetFrameAlpha;
        }

        public bool DoUpdate(float deltaTime)
        {
            if (UnitRenderer.Unit.VisualEffects.HasAnyFlag(UnitVisualEffectFlags.AnyTransparency))
                return false;

            Vector3 targetPosition = UnitRenderer.TagContainer.FindNameplateTag();
            if (targetPosition != transform.position)
                transform.position = targetPosition;

            float distanceToPlayer = renderReference.Player.ExactDistanceTo(UnitRenderer.Unit);

            if (cameraReference.WarcraftCamera != null)
                transform.rotation = Quaternion.LookRotation(cameraReference.WarcraftCamera.transform.forward);

            healthFrame.DoUpdate(deltaTime);
            if (castFrame.gameObject.activeSelf)
                castFrame.DoUpdate();

            if (distanceToPlayer > nameplateSettings.MaxDistance + nameplateSettings.DistanceThreshold)
                return false;

            bool inDetailRange = distanceToPlayer < nameplateSettings.DetailedDistance;
            if (InDetailRange != inDetailRange)
            {
                InDetailRange = inDetailRange;
                UpdateSelection();
            }

            ApplyScaling();

            return true;
        }

        private void ApplyScaling()
        {
            if (!HostilitySettings.ApplyScaling)
                return;

            WarcraftCamera warcraftCamera = cameraReference.WarcraftCamera;
            if (warcraftCamera != null)
            {
                Vector3 direction = transform.position - warcraftCamera.transform.position;
                float distance = Vector3.Dot(direction, warcraftCamera.transform.forward);

                transform.rotation = Quaternion.LookRotation(warcraftCamera.transform.forward);
                contentFrame.transform.localScale = Vector3.one * nameplateSettings.ScaleOverDistance.Evaluate(distance);
            }
        }

        private void Initialize(UnitRenderer unitRenderer)
        {
            UnitRenderer = unitRenderer;

            transform.SetParent(interfaceReference.FindRoot(InterfaceCanvasType.Nameplate));
            transform.position = UnitRenderer.TagContainer.FindNameplateTag();
            unitName.text = unitRenderer.Unit.Name;
            castFrame.UpdateCaster(unitRenderer.Unit);
            healthFrame.Unit = unitRenderer.Unit;
            healthFrame.AlphaTransitionSpeed = nameplateSettings.HealthAlphaTrasitionSpeed;

            OnFactionChanged();

            EventHandler.RegisterEvent(UnitRenderer.Unit, GameEvents.UnitFactionChanged, onFactionChangedAction);
        }

        private void Deinitialize()
        {
            EventHandler.UnregisterEvent(UnitRenderer.Unit, GameEvents.UnitFactionChanged, onFactionChangedAction);

            castFrame.UpdateCaster(null);
            healthFrame.Unit = null;

            UnitRenderer = null;
        }

        private void OnFactionChanged()
        {
            Player referer = renderReference.Player;
            Unit target = UnitRenderer.Unit;

            if (referer == target)
                HostilitySettings = nameplateSettings.Self;
            else if (referer.IsHostileTo(target))
                HostilitySettings = nameplateSettings.Enemy;
            else if (referer.IsFriendlyTo(target))
                HostilitySettings = nameplateSettings.Friendly;
            else
                HostilitySettings = nameplateSettings.Neutral;

            unitName.gameObject.SetActive(HostilitySettings.ShowName);
            healthFrame.HealthBar.FillImage.color = HostilitySettings.HealthColor;
            unitName.color = HostilitySettings.NameWithoutPlateColor;

            InDetailRange = referer.ExactDistanceTo(target) < nameplateSettings.DetailedDistance;

            UpdateSelection(true);

            ApplyScaling();
        }
    }
}
