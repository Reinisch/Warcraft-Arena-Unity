using Client.UI;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Interface Reference", menuName = "Game Data/Scriptable/Interface", order = 10)]
    public class InterfaceReference : ScriptableReference
    {
        [SerializeField, UsedImplicitly] private string containerTag;
        [SerializeField, UsedImplicitly] private LobbyScreen lobbyScreenPrototype;
        [SerializeField, UsedImplicitly] private BattleScreen battleScreenPrototype;

        private readonly ScreenController screenController = new ScreenController();

        private LobbyScreen lobbyScreen;
        private BattleScreen battleScreen;
        private InterfaceContainer container;

        protected override void OnRegistered()
        {
            container = GameObject.FindGameObjectWithTag(containerTag).GetComponent<InterfaceContainer>();
            container.Register();

            lobbyScreen = Instantiate(lobbyScreenPrototype, container.Root);
            battleScreen = Instantiate(battleScreenPrototype, container.Root);

            lobbyScreen.Initialize(screenController);
            battleScreen.Initialize(screenController);
        }

        protected override void OnUnregister()
        {
            lobbyScreen.Deinitialize(screenController);
            battleScreen.Deinitialize(screenController);

            Destroy(lobbyScreen);
            Destroy(battleScreen);

            container.Unregister();
            lobbyScreen = null;
            battleScreen = null;
            container = null;
        }

        protected override void OnUpdate(float deltaTime)
        {
            screenController.DoUpdate(deltaTime);
        }

        internal void AddHandler<TScreen>(IScreenHandler<TScreen> handler) where TScreen : UIPanelController
        {
            screenController.AddHandler(handler);
        }

        internal void RemoveHandler<TScreen>(IScreenHandler<TScreen> handler) where TScreen : UIPanelController
        {
            screenController.RemoveHandler(handler);
        }

        public RectTransform FindRoot(InterfaceCanvasType canvasType) => container.FindRoot(canvasType);

        public void ShowScreen<TScreen, TShowPanel>() where TScreen : UIPanelController where TShowPanel : UIPanel, IPanel<TScreen>
        {
            screenController.ShowScreen<TScreen, TShowPanel>();
        }

        public void ShowScreen<TScreen, TFirstPanel, TPanelShowData>(TPanelShowData showData = default)
            where TScreen : UIPanelController where TFirstPanel : UIPanel, IPanel<TScreen> where TPanelShowData : IPanelShowToken<TFirstPanel>
        {
            screenController.ShowScreen<TScreen, TFirstPanel, TPanelShowData>(showData);
        }

        public void HideScreen<TScreen>() where TScreen : UIPanelController
        {
            screenController.HideScreen<TScreen>();
        }
    }
}