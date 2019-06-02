namespace Client.UI
{
    public interface IPanelInitData
    {
        void Process<TPanelType>(UIPanel<TPanelType> panel);
    }

    public interface IPanelDeinitData
    {
        void Process(IPanel panel);
    }

    public interface IPanelShowData
    {
        void Process(IPanel panel);
    }

    public interface IPanelShowData<out TPanelType> : IPanelShowData
    {
        TPanelType PanelType { get; }
    }

    public interface IPanelHideData
    {
        void Process(IPanel panel);
    }

    public interface IPanelHideData<out TPanelType> : IPanelHideData
    {
        TPanelType PanelType { get; }
    }
}