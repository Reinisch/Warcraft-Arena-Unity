namespace Client.UI
{
    public class UIWindowController<TWindowType> : UIPanelController<UIWindow<TWindowType>, TWindowType>
    {
        public void Show<TPanelShowData>(TPanelShowData firstPanelData) where TPanelShowData : struct, IPanelShowData<TWindowType>
        {
            gameObject.SetActive(true);

            ShowPanel(firstPanelData);
        }

        public void Hide()
        {
            HideAllPanels(new UIWindow<TWindowType>.DefaultHideData());

            gameObject.SetActive(false);
        }
    }
}