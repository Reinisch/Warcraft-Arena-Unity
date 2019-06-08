using System;
using System.Collections.Generic;
using Common;

namespace Client.UI
{
    public sealed class ScreenController
    {
        private readonly List<UIPanelControllerBase> panelControllers = new List<UIPanelControllerBase>();
        private readonly Dictionary<Type, UIPanelControllerBase> panelControllersByPanelType = new Dictionary<Type, UIPanelControllerBase>();
        
        internal void RegisterScreen<TPanel>(UIPanelController<TPanel> panelController) where TPanel : UIPanel
        {
            panelControllersByPanelType.Add(panelController.GetType(), panelController);
            panelControllers.Add(panelController);
        }

        internal void UnregisterScreen<TPanel>(UIPanelController<TPanel> panelController) where TPanel : UIPanel
        {
            panelControllers.Remove(panelController);
            panelControllersByPanelType.Remove(panelController.GetType());
        }

        public void DoUpdate(int deltaTime)
        {
            foreach (var panelController in panelControllers)
                panelController.DoUpdate(deltaTime);
        }
        
        public void ShowScreen<TScreen, TShowPanel>() where TScreen : UIPanelController<UIPanel> where TShowPanel : UIPanel
        {
            Assert.IsTrue(panelControllersByPanelType.ContainsKey(typeof(TScreen)), $"Screen with panel type {typeof(TScreen)} not found when showing!");
            if (!panelControllersByPanelType.TryGetValue(typeof(TScreen), out UIPanelControllerBase basePanelController))
                return;

            if (basePanelController is TScreen panelController)
            {
                panelController.gameObject.SetActive(true);
                panelController.ShowPanel<TShowPanel>();
            }
        }

        public void ShowScreen<TScreen, TShowPanel, TShowToken>(TShowToken token = default)
            where TScreen : UIPanelController<UIPanel> where TShowPanel : UIPanel where TShowToken : IPanelShowToken<TShowPanel>
        {
            Assert.IsTrue(panelControllersByPanelType.ContainsKey(typeof(TScreen)), $"Screen with panel type {typeof(TScreen)} not found when showing!");
            if (!panelControllersByPanelType.TryGetValue(typeof(TScreen), out UIPanelControllerBase basePanelController))
                return;

            if (basePanelController is TScreen panelController)
            {
                panelController.gameObject.SetActive(true);
                panelController.ShowPanel<TShowPanel, TShowToken>(token);
            }
        }

        public void HideScreen<TScreen>()
        {
            Assert.IsTrue(panelControllersByPanelType.ContainsKey(typeof(TScreen)), $"Screen with panel type {typeof(TScreen)} not found when hiding!");
            if (panelControllersByPanelType.TryGetValue(typeof(TScreen), out UIPanelControllerBase basePanelController))
            {
                basePanelController.HideAllPanels();
                basePanelController.gameObject.SetActive(false);
            }
        }
    }
}