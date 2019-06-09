namespace Client.UI
{
    public abstract class UIWindow<T> : UIPanel, IPanel<T> where T : UIWindowController<T> 
    {
        protected UIWindowController<T> WindowController { get; private set; }

        protected override void PanelInitialized()
        {
            base.PanelInitialized();

            WindowController = (UIWindowController<T>)PanelController;
        }

        protected override void PanelDeinitialized()
        {
            WindowController = null;

            base.PanelDeinitialized();
        }
    }
}
