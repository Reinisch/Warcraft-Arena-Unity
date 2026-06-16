using UnityEngine;
using Zenject;

namespace Client.Sound
{
    internal class SoundInstaller : MonoInstaller
    {
        [field: SerializeField]
        private SoundModule Sound;

        public override void InstallBindings()
        {
            Container.BindInstance(Sound).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SoundPlayController>().AsSingle().NonLazy();
        }
    }
}