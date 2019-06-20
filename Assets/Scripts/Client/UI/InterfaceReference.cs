using Client;
using Client.UI;
using Common;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "Interface Reference", menuName = "Game Data/Scriptable/Interface", order = 10)]
public class InterfaceReference : ScriptableReference
{
    [SerializeField, UsedImplicitly] private string containerTag;
    [SerializeField, UsedImplicitly] private LobbyScreen lobbyScreenPrototype;
    [SerializeField, UsedImplicitly] private BattleScreen battleScreenPrototype;

    private readonly ScreenController screenController = new ScreenController();

    private LobbyScreen lobbyScreen;
    private BattleScreen battleScreen;
    private Transform containerTransform;

    protected override void OnRegistered()
    {
        containerTransform = GameObject.FindGameObjectWithTag(containerTag).transform;

        lobbyScreen = Instantiate(lobbyScreenPrototype, containerTransform);
        battleScreen = Instantiate(battleScreenPrototype, containerTransform);

        lobbyScreen.Initialize(screenController);
        battleScreen.Initialize(screenController);
    }

    protected override void OnUnregister()
    {
        lobbyScreen.Deinitialize(screenController);
        battleScreen.Deinitialize(screenController);

        Destroy(lobbyScreen);
        Destroy(battleScreen);

        lobbyScreen = null;
        battleScreen = null;
        containerTransform = null;
    }

    protected override void OnUpdate(float deltaTime)
    {
        screenController.DoUpdate(deltaTime);
    }

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