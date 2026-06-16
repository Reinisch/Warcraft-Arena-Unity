using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Client
{
    public class LobbyClassSlot : MonoBehaviour
    {
        [Inject] private EventBus eventBus;

        [SerializeField, UsedImplicitly] private Button slotButton;
        [SerializeField, UsedImplicitly] private Image selectedFrame;
        [SerializeField, UsedImplicitly] private Image classFrame;
        [SerializeField, UsedImplicitly] private ClassInfo classInfo;

        [UsedImplicitly]
        private void Awake()
        {
            slotButton.onClick.AddListener(OnSlotClicked);
            // Global (untargeted) event so EVERY class slot refreshes when ANY one is picked — a scoped event
            // only reaches the clicked slot, leaving the others' highlights stale (all-selected bug).
            eventBus.RegisterEvent(GameEvents.LobbyClassChanged, OnLobbyClassChanged);
        }

        [UsedImplicitly]
        private void Start()
        {
            UpdateSelection();
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            eventBus.UnregisterEvent(GameEvents.LobbyClassChanged, OnLobbyClassChanged);
            slotButton.onClick.RemoveListener(OnSlotClicked);
        }

        private void UpdateSelection()
        {
            // Default to Mage when nothing is chosen yet, so a slot is always highlighted (and matches the
            // Mage fallback used at spawn).
            int selectedClass = PlayerPrefs.GetInt(UnitUtils.PreferredClassPrefName, (int)ClassType.Mage);
            selectedFrame.enabled = selectedClass == (int)classInfo.ClassType;
        }

        private void OnSlotClicked()
        {
            PlayerPrefs.SetInt(UnitUtils.PreferredClassPrefName, (int)classInfo.ClassType);

            eventBus.ExecuteEvent(GameEvents.LobbyClassChanged);
        }

        private void OnLobbyClassChanged() => UpdateSelection();
    }
}
