using System;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class LobbyMapSlot : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private Button slotButton;
        [SerializeField, UsedImplicitly] private Image selectedFrame;
        [SerializeField, UsedImplicitly] private Image mapFrame;

        public event Action<LobbyMapSlot> EventLobbyMapSlotSelected;

        public ScenarioDefinition ScenarioDefiniton { get; private set; }

        public void Initialize(ScenarioDefinition scenarioDefiniton)
        {
            gameObject.SetActive(true);

            ScenarioDefiniton = scenarioDefiniton;
            mapFrame.sprite = scenarioDefiniton.Map.SlotBackground;
            slotButton.interactable = scenarioDefiniton.Map.IsAvailable;

            slotButton.onClick.AddListener(OnMapSlotClicked);
        }

        public void Deinitialize()
        {
            ScenarioDefiniton = null;

            slotButton.onClick.RemoveListener(OnMapSlotClicked);
        }

        public void SetSelectState(bool isSelected)
        {
            selectedFrame.enabled = isSelected;
        }

        public void Select()
        {
            EventLobbyMapSlotSelected?.Invoke(this);
        }

        private void OnMapSlotClicked()
        {
            Select();
        }
    }
}
