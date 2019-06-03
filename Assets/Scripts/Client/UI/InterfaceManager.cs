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

        EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
        EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
    }

    public new void Deinitialize()
    {
        EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
        EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);

        battleScreen.Deinitialize();
        lobbyScreen.Deinitialize();

        base.Deinitialize();
    }

    public void DoUpdate(int deltaTime)
    {
        lobbyScreen.DoUpdate(deltaTime);
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
        battleScreen.Show(new UIWindow<BattlePanelType>.DefaultShowData(BattlePanelType.HudPanel));
    }

    public void HideBattleScreen()
    {
        battleScreen.Hide();
    }

    private void OnWorldInitialized(WorldManager worldManager)
    {
        if (worldManager.HasClientLogic)
        {
            battleScreen.InitializeWorld(worldManager);
        }
    }

    private void OnWorldDeinitializing(WorldManager worldManager)
    {
        if (worldManager.HasClientLogic)
        {
            battleScreen.DeinitializeWorld();
        }
    }
}