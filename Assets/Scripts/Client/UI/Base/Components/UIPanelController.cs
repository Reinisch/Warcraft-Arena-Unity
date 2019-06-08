using System;
using System.Collections.Generic;
using Common;

namespace Client.UI
{
    public abstract class UIPanelController<TPanel> : UIPanelControllerBase where TPanel : UIPanel
    {
        private readonly Dictionary<Type, TPanel> panelsByType = new Dictionary<Type, TPanel>();
        private readonly List<TPanel> panels = new List<TPanel>();

        protected void Initialize(ScreenController screenController)
        {
            screenController.RegisterScreen(this);
        }

        protected void Deinitialize(ScreenController screenController)
        {
            screenController.UnregisterScreen(this);
        }

        protected void RegisterPanel<TRegisterPanel, TRegisterToken>(TRegisterPanel panel, TRegisterToken token = default)
            where TRegisterToken : struct, IPanelRegisterToken<TRegisterPanel> where TRegisterPanel : TPanel
        {
            token.Initialize(panel);
            panel.Initialize();

            panelsByType.Add(panel.GetType(), panel);
            panels.Add(panel);
        }

        protected void UnregisterPanel<TUnregisterPanel, TUnregisterToken>(TUnregisterPanel panel, TUnregisterToken token = default)
            where TUnregisterToken : struct, IPanelUnregisterToken<TUnregisterPanel> where TUnregisterPanel : TPanel
        {
            panels.Remove(panel);
            panelsByType.Remove(panel.GetType());

            panel.Deinitialize();
            token.Deinitialize(panel);
        }

        public void ShowPanel<TShowPanel>() where TShowPanel : TPanel
        {
            if (panelsByType.TryGetValue(typeof(TShowPanel), out TPanel panel) && panel is TShowPanel showPanel)
                showPanel.Show();
            else
                Assert.IsTrue(false, $"Panel {typeof(TShowPanel)} was not found when showing or has invalid type!");
        }

        public void ShowPanel<TShowPanel, TShowToken>(TShowToken token = default) where TShowPanel : TPanel where TShowToken : IPanelShowToken<TShowPanel>
        {
            if (panelsByType.TryGetValue(typeof(TShowPanel), out TPanel panel) && panel is TShowPanel showPanel)
            {
                token.Process(showPanel);
                showPanel.Show();
            }
            else
                Assert.IsTrue(false, $"Panel {typeof(TShowPanel)} not found when showing or has invalid type!");
        }

        public void HidePanel<THidePanel>() where THidePanel : TPanel
        {
            if (panelsByType.TryGetValue(typeof(THidePanel), out TPanel panel) && panel is THidePanel hidePanel)
                hidePanel.Hide();
            else
                Assert.IsTrue(false, $"Panel {typeof(THidePanel)} not found when hiding or has invalid type!");
        }
        
        public void HidePanel<THidePanel, THideToken>(THideToken token) where THideToken : struct, IPanelHideToken<THidePanel> where THidePanel : TPanel
        {
            if (panelsByType.TryGetValue(typeof(THidePanel), out TPanel panel) && panel is THidePanel hidePanel)
            {
                token.Process(hidePanel);
                panel.Hide();
            }
            else
                Assert.IsTrue(false, $"Panel {typeof(THidePanel)} not found when hiding or has invalid type!");
        }

        public override void HideAllPanels()
        {
            for (int i = panels.Count - 1; i >= 0; i--)
                panels[i].Hide();
        }

        internal override void DoUpdate(int deltaTime)
        {
            for (int i = panels.Count - 1; i >= 0; i--)
                panels[i].DoUpdate(deltaTime);
        }
    }
}
