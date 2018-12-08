using System.Collections.Generic;
using Client;
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

    public void Initialize(PhotonBoltManager photonManager)
    {
        defaultActionBars.ForEach(actionBar => loadedBehaviours.Add(actionBar));
        loadedBehaviours.ForEach(behaviour => behaviour.Initialize());

        lobbyScreen.Initialize(photonManager);
        battleScreen.Initialize(photonManager);
    }

    public void Deinitialize()
    {
        battleScreen.Deinitialize();
        lobbyScreen.Deinitialize();

        loadedBehaviours.ForEach(behaviour => behaviour.Deinitialize());
        defaultActionBars.ForEach(actionBar => loadedBehaviours.Remove(actionBar));
    }

    public void DoUpdate(int deltaTime)
    {
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