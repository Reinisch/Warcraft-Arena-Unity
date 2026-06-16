using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Client.UI
{
    public abstract class UIScreen : MonoBehaviour
    {
        internal readonly List<UIPanel> Panels = new();

        internal void Register()
        {
            ScreenInitialized();
        }

        internal void Unregister()
        {
            ScreenDeinitialized();
        }

        protected virtual void ScreenInitialized()
        {
        }

        protected virtual void ScreenDeinitialized()
        {
        }

        internal void DoUpdate(float deltaTime)
        {
            for (int i = Panels.Count - 1; i >= 0; i--)
                Panels[i].DoUpdate(deltaTime);
        }

        internal TPanel FindPanel<TPanel>() where TPanel : UIPanel
        {
            for (int i = 0; i < Panels.Count; i++)
                if (Panels[i] is TPanel panel)
                    return panel;

            return null;
        }

        public bool IsPanelShown<TPanel>() where TPanel : UIPanel
        {
            return FindPanel<TPanel>() is { } panel && panel.IsShown;
        }

        public void HideAllPanels()
        {
            for (int i = Panels.Count - 1; i >= 0; i--)
                Panels[i].Hide();
        }
    }

    public abstract class UIScreen<T> : UIScreen where T : UIScreen<T>
    {
        [SerializeField]
        private List<UIPanel<T>> panels = new();

        protected override void ScreenInitialized()
        {
            foreach (var panel in panels)
            {
                panel.Register((T)this);
                Panels.Add(panel);
            }

            gameObject.SetActive(false);
        }

        protected override void ScreenDeinitialized()
        {
            foreach (var panel in panels)
            {
                panel.Unregister();
                Panels.Remove(panel);
            }

            gameObject.SetActive(false);
        }

        public void ShowPanel<TShowPanel>()
            where TShowPanel : UIPanel<T>
        {
            if (FindPanel<TShowPanel>() is { } showPanel)
                showPanel.Show();
            else
                Assert.Fail($"Panel {typeof(TShowPanel)} was not found when showing or has invalid type!");
        }

        public void ShowPanel<TShowPanel, TShowToken>(TShowToken token = default)
            where TShowPanel : UIPanel<T>
            where TShowToken : IPanelShowToken<TShowPanel>
        {
            if (FindPanel<TShowPanel>() is { } showPanel)
            {
                showPanel.Show();
                token.Process(showPanel);
            }
            else
                Assert.Fail($"Panel {typeof(TShowPanel)} not found when showing or has invalid type!");
        }

        public void HidePanel<THidePanel>()
            where THidePanel : UIPanel<T>
        {
            if (FindPanel<THidePanel>() is { } hidePanel)
                hidePanel.Hide();
            else
                Assert.Fail($"Panel {typeof(THidePanel)} not found when hiding or has invalid type!");
        }

        public void HidePanel<THidePanel, THideToken>(THideToken token = default)
            where THideToken : struct, IPanelHideToken<THidePanel> 
            where THidePanel : UIPanel<T>
        {
            if (FindPanel<THidePanel>() is { } hidePanel)
            {
                token.Process(hidePanel);
                hidePanel.Hide();
            }
            else
                Assert.Fail($"Panel {typeof(THidePanel)} not found when hiding or has invalid type!");
        }
    }
}
