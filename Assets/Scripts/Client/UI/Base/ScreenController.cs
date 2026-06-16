using Common;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Client.UI
{
    public abstract class ScreenController: ScriptableReference
    {
        [SerializeField, UsedImplicitly] private RectTransform root;
        [SerializeField, UsedImplicitly] private List<UIScreen> prototypes;

        [Inject]
        private DiContainer diContainer;

        private readonly List<UIScreen> screens = new();
        private readonly Dictionary<Type, UIScreen> screensByType = new();
        private readonly Dictionary<Type, List<IScreenHandler>> screenHandlersByType = new();

        public RectTransform Root => root;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            foreach(var screenPrototype in prototypes)
            {
                UIScreen screen = diContainer.InstantiatePrefab(screenPrototype, Root).GetComponent<UIScreen>();
                screensByType.Add(screen.GetType(), screen);
                screens.Add(screen);
                screen.Register();
            }
        }

        protected override void OnUnregister()
        {
            foreach (var screen in screens)
            {
                screen.Unregister();
                Destroy(screen.gameObject);
            }

            screens.Clear();
            screensByType.Clear();

            base.OnUnregister();
        }

        protected override void OnUpdate(float deltaTime)
        {
            foreach (var screen in screens)
                screen.DoUpdate(deltaTime);
        }

        public void AddHandler<TScreen>(IScreenHandler<TScreen> handler) where TScreen : UIScreen<TScreen>
        {
            if (!screenHandlersByType.TryGetValue(typeof(TScreen), out List<IScreenHandler> handlers))
            {
                handlers = new List<IScreenHandler>();
                screenHandlersByType.Add(typeof(TScreen), handlers);
            }

            handlers.Add(handler);

            if (screensByType.TryGetValue(typeof(TScreen), out UIScreen screen))
                handler.OnScreenShown((TScreen)screen);
        }

        public void RemoveHandler<TScreen>(IScreenHandler<TScreen> handler) where TScreen : UIScreen<TScreen>
        {
            if (screenHandlersByType.TryGetValue(typeof(TScreen), out List<IScreenHandler> handlers))
                handlers.Remove(handler);

            if (screensByType.TryGetValue(typeof(TScreen), out UIScreen screen))
                handler.OnScreenHide((TScreen)screen);
        }

        public void ShowScreen<TScreen, TShowPanel>() where TScreen : UIScreen<TScreen> where TShowPanel : UIPanel<TScreen>
        {
            Assert.IsTrue(screensByType.ContainsKey(typeof(TScreen)), $"Screen with panel type {typeof(TScreen)} not found when showing!");
            if (!screensByType.TryGetValue(typeof(TScreen), out UIScreen baseScreen))
                return;

            if (baseScreen is TScreen screen)
            {
                screen.gameObject.SetActive(true);
                screen.ShowPanel<TShowPanel>();

                HandleScreenShown(screen);
            }
        }

        public void ShowScreen<TScreen, TShowPanel, TShowToken>(TShowToken token = default)
            where TScreen : UIScreen<TScreen> where TShowPanel : UIPanel<TScreen> where TShowToken : IPanelShowToken<TShowPanel>
        {
            Assert.IsTrue(screensByType.ContainsKey(typeof(TScreen)), $"Screen with panel type {typeof(TScreen)} not found when showing!");
            if (!screensByType.TryGetValue(typeof(TScreen), out UIScreen baseScreen))
                return;

            if (baseScreen is TScreen screen)
            {
                screen.gameObject.SetActive(true);
                screen.ShowPanel<TShowPanel, TShowToken>(token);

                HandleScreenShown(screen);
            }
        }

        public bool IsPanelShown<TScreen, TPanel>()
            where TScreen : UIScreen<TScreen>
            where TPanel : UIPanel<TScreen>
        {
            return screensByType.TryGetValue(typeof(TScreen), out UIScreen screen) && screen.IsPanelShown<TPanel>();
        }

        public bool IsScreenShown<TScreen>() where TScreen : UIScreen<TScreen>
        {
            return screensByType.TryGetValue(typeof(TScreen), out UIScreen screen) && screen.gameObject.activeSelf;
        }

        public void HideScreen<TScreen>() where TScreen : UIScreen<TScreen>
        {
            Assert.IsTrue(screensByType.ContainsKey(typeof(TScreen)), $"Screen with panel type {typeof(TScreen)} not found when hiding!");
            if (screensByType.TryGetValue(typeof(TScreen), out UIScreen baseScreen) && baseScreen is TScreen screen)
            {
                screen.HideAllPanels();
                screen.gameObject.SetActive(false);

                HandleScreenHide(screen);
            }
        }

        private void HandleScreenShown<TScreen>(TScreen screen) where TScreen : UIScreen
        {
            if (screenHandlersByType.TryGetValue(typeof(TScreen), out List<IScreenHandler> handlers))
                for (int i = 0; i < handlers.Count; i++)
                    ((IScreenHandler<TScreen>)handlers[i]).OnScreenShown(screen);
        }

        private void HandleScreenHide<TScreen>(TScreen screen) where TScreen : UIScreen
        {
            if (screenHandlersByType.TryGetValue(typeof(TScreen), out List<IScreenHandler> handlers))
                for (int i = 0; i < handlers.Count; i++)
                    ((IScreenHandler<TScreen>)handlers[i]).OnScreenHide(screen);
        }
    }
}
