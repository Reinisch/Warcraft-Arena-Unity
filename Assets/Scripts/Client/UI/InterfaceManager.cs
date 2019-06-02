using Client;
using Client.UI;
using Common;
using JetBrains.Annotations;
using UnityEngine;
using Core;

public class InterfaceManager : SingletonBehaviour<InterfaceManager>
{
    [SerializeField, UsedImplicitly] private LobbyScreen lobbyScreen;
    [SerializeField, UsedImplicitly] private BattleScreen battleScreen;

    public void Initialize(PhotonBoltManager photonManager, PhotonBoltClientListener clientListener)
    {
        base.Initialize();

        lobbyScreen.Initialize(photonManager);
        battleScreen.Initialize(photonManager, clientListener);
    }

    public new void Deinitialize()
    {
        battleScreen.Deinitialize();
        lobbyScreen.Deinitialize();

        base.Deinitialize();
    }

    public void InitializeWorld(WorldManager worldManager)
    {
        battleScreen.InitializeWorld(worldManager);
    }

    public void DeinitializeWorld()
    {
        battleScreen.DeinitializeWorld();
    }

    public void DoUpdate(int deltaTime)
    {
        battleScreen.DoUpdate(deltaTime);
    }

    public void ShowLobbyScreen<TData>(TData showData) where TData : struct, IPanelShowData<LobbyPanelType> 
    {
        lobbyScreen.Show(showData);
    }

    public void HideLobbyScreen()
    {
        lobbyScreen.Hide();
    }

    public void ShowBattleScreen()
    {
        battleScreen.Show();
    }

    public void HideBattleScreen()
    {
        battleScreen.Hide();
    }
}