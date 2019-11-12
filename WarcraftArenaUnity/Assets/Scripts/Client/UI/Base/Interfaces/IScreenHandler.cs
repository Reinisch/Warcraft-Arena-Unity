namespace Client
{
    public interface IScreenHandler<in TScreen> : IScreenHandler
    {
        void OnScreenShown(TScreen screen);
        void OnScreenHide(TScreen screen);
    }

    public interface IScreenHandler
    {
    }
}
