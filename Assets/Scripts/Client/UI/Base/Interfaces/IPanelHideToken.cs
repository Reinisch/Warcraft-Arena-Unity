namespace Client.UI
{
    public interface IPanelHideToken<in TPanel>
    {
        void Process(TPanel panel);
    }
}