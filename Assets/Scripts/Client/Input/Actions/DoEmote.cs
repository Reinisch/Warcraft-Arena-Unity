using Core;
using UnityEngine;
using JetBrains.Annotations;
using Zenject;

namespace Client.Actions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action - Do Emote", menuName = "Player Data/Input/Actions/Do Emote", order = 2)]
    public class DoEmote : InputAction
    {
        [Inject] private InputReference input;
        [SerializeField, UsedImplicitly] private EmoteType emoteType;

        public override void Execute() => input.DoEmote(emoteType);
    }
}
