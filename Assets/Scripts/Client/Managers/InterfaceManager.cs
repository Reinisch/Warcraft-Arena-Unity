using System.Collections.Generic;
using Client;
using Common;
using JetBrains.Annotations;
using UnityEngine;
using Core;

public class InterfaceManager : SingletonGameObject<InterfaceManager>
{
    [SerializeField, UsedImplicitly] private LobbyScreen lobbyScreen;
    [SerializeField, UsedImplicitly] private BattleScreen battleScreen;
    [SerializeField, UsedImplicitly] private ButtonController buttonController;
    [SerializeField, UsedImplicitly] private List<ActionBar> defaultActionBars;

    private readonly List<UIBehaviour> loadedBehaviours = new List<UIBehaviour>();

    public ButtonController ButtonController => buttonController;
    public LobbyScreen LobbyScreen => lobbyScreen;

    public void Initialize(PhotonBoltManager photonManager, PhotonBoltClientListener clientListener)
    {
        base.Initialize();

        defaultActionBars.ForEach(actionBar => loadedBehaviours.Add(actionBar));
        loadedBehaviours.ForEach(behaviour => behaviour.Initialize());

        lobbyScreen.Initialize(photonManager);
        battleScreen.Initialize(photonManager, clientListener);
    }

    public new void Deinitialize()
    {
        battleScreen.Deinitialize();
        lobbyScreen.Deinitialize();

        loadedBehaviours.ForEach(behaviour => behaviour.Deinitialize());
        defaultActionBars.ForEach(actionBar => loadedBehaviours.Remove(actionBar));

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

    public void ShowLobbyScreen(bool autoStartClient)
    {
        lobbyScreen.Show(autoStartClient);
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