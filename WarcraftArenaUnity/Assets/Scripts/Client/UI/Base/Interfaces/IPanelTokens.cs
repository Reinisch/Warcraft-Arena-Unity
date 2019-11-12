namespace Client.UI
{
    public interface IPanelRegisterToken<in TPanel> where TPanel : UIPanel
    {
        void Initialize(TPanel panel);
    }

    public interface IPanelUnregisterToken<in TPanel> where TPanel : UIPanel
    {
        void Deinitialize(TPanel panel);
    }

    public interface IPanelShowToken<in TPanel> where TPanel : UIPanel
    {
        void Process(TPanel panel);
    }

    public interface IPanelHideToken<in TPanel> where TPanel : UIPanel
    {
        void Process(TPanel panel);
    }
}