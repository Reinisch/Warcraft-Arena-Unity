using System;
using System.Collections.Generic;
using Common;

namespace Client.UI
{
    public sealed class ScreenController
    {
        private readonly List<UIPanelController> panelControllers = new List<UIPanelController>();
        private readonly List<IScreenHandler> screenHandlers = new List<IScreenHandler>();
        private readonly Dictionary<Type, UIPanelController> panelControllersByPanelType = new Dictionary<Type, UIPanelController>();

        internal void RegisterScreen(UIPanelController panelController)
        {
            panelControllersByPanelType.Add(panelController.GetType(), panelController);
            panelControllers.Add(panelController);
        }

        internal void UnregisterScreen(UIPanelController panelController)
        {
            panelControllers.Remove(panelController);
            panelControllersByPanelType.Remove(panelController.GetType());
        }

        public void AddHandler<TScreen>(IScreenHandler<TScreen> handler) where TScreen : UIPanelController
        {
            screenHandlers.Add(handler);

            if (panelControllersByPanelType.TryGetValue(typeof(TScreen), out UIPanelController basePanelController))
                handler.OnScreenShown((TScreen)basePanelController);
        }

        public void RemoveHandler<TScreen>(IScreenHandler<TScreen> handler) where TScreen : UIPanelController
        {
            screenHandlers.Remove(handler);

            if (panelControllersByPanelType.TryGetValue(typeof(TScreen), out UIPanelController basePanelController))
                handler.OnScreenHide((TScreen)basePanelController);
        }

        public void DoUpdate(float deltaTime)
        {
            foreach (var panelController in panelControllers)
                panelController.DoUpdate(deltaTime);
        }
        
        public void ShowScreen<TScreen, TShowPanel>() where TScreen : UIPanelController where TShowPanel : UIPanel, IPanel<TScreen>
        {
            Assert.IsTrue(panelControllersByPanelType.ContainsKey(typeof(TScreen)), $"Screen with panel type {typeof(TScreen)} not found when showing!");
            if (!panelControllersByPanelType.TryGetValue(typeof(TScreen), out UIPanelController basePanelController))
                return;

            if (basePanelController is TScreen panelController)
            {
                panelController.gameObject.SetActive(true);
                panelController.ShowPanelInternal<TShowPanel>();

                HandleScreenShown(panelController);
            }
        }

        public void ShowScreen<TScreen, TShowPanel, TShowToken>(TShowToken token = default)
            where TScreen : UIPanelController where TShowPanel : UIPanel where TShowToken : IPanelShowToken<TShowPanel>
        {
            Assert.IsTrue(panelControllersByPanelType.ContainsKey(typeof(TScreen)), $"Screen with panel type {typeof(TScreen)} not found when showing!");
            if (!panelControllersByPanelType.TryGetValue(typeof(TScreen), out UIPanelController basePanelController))
                return;

            if (basePanelController is TScreen panelController)
            {
                panelController.gameObject.SetActive(true);
                panelController.ShowPanelInternal<TShowPanel, TShowToken>(token);

                HandleScreenShown(panelController);
            }
        }

        public void HideScreen<TScreen>() where TScreen : UIPanelController
        {
            Assert.IsTrue(panelControllersByPanelType.ContainsKey(typeof(TScreen)), $"Screen with panel type {typeof(TScreen)} not found when hiding!");
            if (panelControllersByPanelType.TryGetValue(typeof(TScreen), out UIPanelController basePanelController))
            {
                basePanelController.HideAllPanels();
                basePanelController.gameObject.SetActive(false);

                HandleScreenHide(basePanelController);
            }
        }

        private void HandleScreenShown<TScreen>(TScreen screen)
        {
            foreach (var screenHandler in screenHandlers)
                if (screenHandler is IScreenHandler<TScreen> targetScreenHandler)
                    targetScreenHandler.OnScreenShown(screen);
        }

        private void HandleScreenHide<TScreen>(TScreen screen)
        {
            foreach (var screenHandler in screenHandlers)
                if (screenHandler is IScreenHandler<TScreen> targetScreenHandler)
                    targetScreenHandler.OnScreenHide(screen);
        }
    }
}