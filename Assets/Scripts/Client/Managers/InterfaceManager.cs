using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Core;

public class InterfaceManager : SingletonGameObject<InterfaceManager>
{
    [SerializeField, UsedImplicitly] private List<ActionBar> defaultActionBars;
    private readonly List<UIBehaviour> loadedBehaviours = new List<UIBehaviour>();

    public ButtonController ButtonController { get; private set; }

    public void Initialize()
    {
        defaultActionBars.ForEach(actionBar => loadedBehaviours.Add(actionBar));
        loadedBehaviours.ForEach(behaviour => behaviour.Initialize());
    }

    public void Deinitialize()
    {
        loadedBehaviours.ForEach(behaviour => behaviour.Deinitialize());
        defaultActionBars.ForEach(actionBar => loadedBehaviours.Remove(actionBar));
    }

    public void DoUpdate(int deltaTime)
    {
    }
}