using UnityEngine;
using System.Collections.Generic;

namespace Client.UI
{
    public abstract class UIPanelController<TPanel, TPanelType> : MonoBehaviour where TPanel : UIPanel<TPanelType>
    {
        private readonly Dictionary<TPanelType, TPanel> panelsByType = new Dictionary<TPanelType, TPanel>();
        private readonly List<TPanel> panels = new List<TPanel>();

        protected void RegisterPanel<TPanelInitData>(TPanel panel, TPanelInitData initData = default) where TPanelInitData : struct, IPanelInitData
        {
            panel.Initialize(initData);

            panelsByType.Add(panel.PanelType, panel);
            panels.Add(panel);
        }

        protected void UnregisterPanel<TPanelDeinitData>(TPanel panel, TPanelDeinitData deinitData = default) where TPanelDeinitData : struct, IPanelDeinitData
        {
            panels.Remove(panel);
            panelsByType.Remove(panel.PanelType);

            panel.Deinitialize(deinitData);
        }

        public void ShowPanel<TPanelShowData>(TPanelShowData panelData) where TPanelShowData : struct, IPanelShowData<TPanelType>
        {
            if (panelsByType.TryGetValue(panelData.PanelType, out var screen))
                screen.Show(panelData);
        }

        public void HidePanel<TPanelHideData>(TPanelHideData panelData) where TPanelHideData : struct, IPanelHideData<TPanelType>
        {
            if (panelsByType.TryGetValue(panelData.PanelType, out var screen))
                screen.Hide(panelData);
        }

        public void HideAllPanels<TPanelHideData>(TPanelHideData panelData) where TPanelHideData : struct, IPanelHideData
        {
            for (int i = panels.Count - 1; i >= 0; i--)
                panels[i].Hide(panelData);
        }
    }
}
