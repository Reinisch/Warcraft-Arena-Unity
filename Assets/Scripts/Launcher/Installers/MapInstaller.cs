using Core;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Workflow
{
    internal class MapInstaller: MonoInstaller
    {
        [field: SerializeField]
        private MapSettings MapSettings;

        public override void InstallBindings()
        {
            Container.BindInstance(MapSettings);
            Container.BindInterfacesAndSelfTo<Map>().AsSingle().NonLazy();
        }
    }
}
