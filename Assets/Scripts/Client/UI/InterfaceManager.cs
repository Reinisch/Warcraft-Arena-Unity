using Client;
using Client.UI;
using JetBrains.Annotations;
using UnityEngine;
using Core;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField, UsedImplicitly] private LobbyScreen lobbyScreen;
    [SerializeField, UsedImplicitly] private BattleScreen battleScreen;

    private readonly ScreenController screenController = new ScreenController();

    public void Initialize(PhotonBoltManager photonManager, PhotonBoltClientListener clientListener)
    {
        lobbyScreen.Initialize(photonManager, screenController);
        battleScreen.Initialize(clientListener, screenController);
    }

    public void Deinitialize()
    {
        battleScreen.Deinitialize(screenController);
        lobbyScreen.Deinitialize(screenController);
    }

    public void DoUpdate(float deltaTime)
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