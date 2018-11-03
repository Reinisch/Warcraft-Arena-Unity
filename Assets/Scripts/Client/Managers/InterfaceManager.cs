using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Client;
using Core;

public class InterfaceManager : SingletonGameObject<InterfaceManager>
{
    [SerializeField, UsedImplicitly] private List<ActionBar> defaultActionBars;
    private readonly List<UIBehaviour> loadedBehaviours = new List<UIBehaviour>();

    public ButtonController ButtonController { get; private set; }
    public UnitFrame playerFrameNewUI;
    public UnitFrame targetFrameNewUI;
    public BuffBar playerBuffsNewUI;
    public BuffBar targetBuffsNewUI;
    public CastFrame playerCastFrameNewUI;

    public override void Initialize()
    {
        base.Initialize();

        defaultActionBars.ForEach(actionBar => loadedBehaviours.Add(actionBar));
        loadedBehaviours.ForEach(behaviour => behaviour.Initialize());
    }

    public override void Deinitialize()
    {
        loadedBehaviours.ForEach(behaviour => behaviour.Deinitialize());
        defaultActionBars.ForEach(actionBar => loadedBehaviours.Remove(actionBar));

        base.Deinitialize();
    }

    public override void DoUpdate(int deltaTime)
    {
        base.DoUpdate(deltaTime);

        if (InputManager.Instance.OriginalPlayer == null)
            return;
    }
}