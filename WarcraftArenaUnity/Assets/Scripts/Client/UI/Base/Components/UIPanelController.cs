using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Client.UI
{
    public abstract class UIPanelController : MonoBehaviour
    {
        private readonly Dictionary<Type, UIPanel> panelsByType = new Dictionary<Type, UIPanel>();

        internal readonly List<UIPanel> Panels = new List<UIPanel>();

        protected void Initialize(ScreenController screenController)
        {
            screenController.RegisterScreen(this);
        }

        protected void Deinitialize(ScreenController screenController)
        {
            screenController.UnregisterScreen(this);
        }

        protected void RegisterPanel<TRegisterPanel, TRegisterToken>(TRegisterPanel panel, TRegisterToken token = default)
            where TRegisterToken : struct, IPanelRegisterToken<TRegisterPanel> where TRegisterPanel : UIPanel
        {
            token.Initialize(panel);
            panel.Initialize(this);

            panelsByType.Add(panel.GetType(), panel);
            Panels.Add(panel);
        }

        protected void UnregisterPanel<TUnregisterPanel, TUnregisterToken>(TUnregisterPanel panel, TUnregisterToken token = default)
            where TUnregisterToken : struct, IPanelUnregisterToken<TUnregisterPanel> where TUnregisterPanel : UIPanel
        {
            Panels.Remove(panel);
            panelsByType.Remove(panel.GetType());

            panel.Deinitialize();
            token.Deinitialize(panel);
        }

        internal void ShowPanelInternal<TShowPanel>() where TShowPanel : UIPanel
        {
            if (panelsByType.TryGetValue(typeof(TShowPanel), out UIPanel panel) && panel is TShowPanel showPanel)
                showPanel.Show();
            else
                Assert.IsTrue(false, $"Panel {typeof(TShowPanel)} was not found when showing or has invalid type!");
        }

        internal void ShowPanelInternal<TShowPanel, TShowToken>(TShowToken token = default) where TShowPanel : UIPanel where TShowToken : IPanelShowToken<TShowPanel>
        {
            if (panelsByType.TryGetValue(typeof(TShowPanel), out UIPanel panel) && panel is TShowPanel showPanel)
            {
                showPanel.Show();
                token.Process(showPanel);
            }
            else
                Assert.IsTrue(false, $"Panel {typeof(TShowPanel)} not found when showing or has invalid type!");
        }

        internal void HidePanelInternal<THidePanel>() where THidePanel : UIPanel
        {
            if (panelsByType.TryGetValue(typeof(THidePanel), out UIPanel panel) && panel is THidePanel hidePanel)
                hidePanel.Hide();
            else
                Assert.IsTrue(false, $"Panel {typeof(THidePanel)} not found when hiding or has invalid type!");
        }

        internal void HidePanelInternal<THidePanel, THideToken>(THideToken token = default) where THideToken : struct, IPanelHideToken<THidePanel> where THidePanel : UIPanel
        {
            if (panelsByType.TryGetValue(typeof(THidePanel), out UIPanel panel) && panel is THidePanel hidePanel)
            {
                token.Process(hidePanel);
                panel.Hide();
            }
            else
                Assert.IsTrue(false, $"Panel {typeof(THidePanel)} not found when hiding or has invalid type!");
        }

        internal void HideAllPanels()
        {
            for (int i = Panels.Count - 1; i >= 0; i--)
                Panels[i].Hide();
        }

        internal abstract void DoUpdate(float deltaTime);
    }
}
