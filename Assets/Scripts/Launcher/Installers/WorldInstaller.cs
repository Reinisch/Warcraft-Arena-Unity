using Common;
using Core;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Workflow
{
    internal class WorldInstaller: MonoInstaller
    {
        [field: SerializeField]
        private WorldSession Session;

        [field: SerializeField]
        private PlayerManager PlayerManager;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<World>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ProjectileLauncher>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SpellManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UnitManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WorldEntityFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameObjectFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MapController>().AsSingle().NonLazy();
            Container.BindInstance(PlayerManager);
            Container.BindInstance(Session);
        }
    }
}
