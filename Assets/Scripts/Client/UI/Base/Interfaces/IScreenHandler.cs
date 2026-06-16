using Client.UI;

namespace Client
{
    public interface IScreenHandler
    {
    }

    public interface IScreenHandler<in TScreen> : IScreenHandler where TScreen: UIScreen
    {
        void OnScreenShown(TScreen screen);

        void OnScreenHide(TScreen screen);
    }
}
