namespace Client.UI
{
    public interface IPanelShowToken<in TPanel>
    {
        void Process(TPanel panel);
    }
}