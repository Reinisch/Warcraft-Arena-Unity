namespace Client.UI
{
    public class UIWindowController<T> : UIPanelController where T : UIWindowController<T>
    {
        internal override void DoUpdate(float deltaTime)
        {
            for (int i = Panels.Count - 1; i >= 0; i--)
                Panels[i].DoUpdate(deltaTime);
        }

        public void ShowPanel<TShowPanel>() where TShowPanel : UIWindow<T>
        {
            ShowPanelInternal<TShowPanel>();
        }

        public void ShowPanel<TShowPanel, TShowToken>(TShowToken token = default) where TShowPanel : UIWindow<T> where TShowToken : IPanelShowToken<TShowPanel>
        {
            ShowPanelInternal<TShowPanel, TShowToken>(token);
        }

        public void HidePanel<THidePanel>() where THidePanel : UIWindow<T>
        {
            HidePanelInternal<THidePanel>();
        }

        public void HidePanel<THidePanel, THideToken>(THideToken token = default) where THidePanel : UIWindow<T> where THideToken : struct, IPanelHideToken<THidePanel>
        {
            HidePanelInternal<THidePanel, THideToken>(token);
        }
    }
}