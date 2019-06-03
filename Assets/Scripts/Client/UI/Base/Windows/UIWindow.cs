namespace Client.UI
{
    public abstract class UIWindow<TPanelType> : UIPanel<TPanelType>
    {
        public struct DefaultShowData : IPanelShowData<TPanelType>
        {
            public TPanelType PanelType { get; }

            public DefaultShowData(TPanelType panelType)
            {
                PanelType = panelType;
            }

            public void Process(IPanel panel)
            {
                panel.GameObject.SetActive(true);
            }
        }

        public struct DefaultHideData : IPanelHideData
        {
            public void Process(IPanel panel)
            {
                panel.GameObject.SetActive(false);
            }
        }

        public struct DefaultDeinitData : IPanelDeinitData
        {
            public void Process(IPanel panel)
            {
                panel.GameObject.SetActive(false);
            }
        }
    }
}
