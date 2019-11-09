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

        public MapDefinition MapDefinition { get; private set; }

        public void Initialize(MapDefinition mapDefiniton)
        {
            gameObject.SetActive(true);

            MapDefinition = mapDefiniton;
            mapFrame.sprite = mapDefiniton.SlotBackground;
            slotButton.interactable = MapDefinition.IsAvailable;

            slotButton.onClick.AddListener(OnMapSlotClicked);
        }

        public void Deinitialize()
        {
            MapDefinition = null;

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
