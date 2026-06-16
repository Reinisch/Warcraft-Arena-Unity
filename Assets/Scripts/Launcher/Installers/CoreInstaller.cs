using Common;
using Core;
using Game;
using Net.Ngo;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Workflow
{
    internal class CoreInstaller: MonoInstaller
    {
        [field: SerializeField]
        private WorldTickableManager TickableManager;

        [field: SerializeField]
        private GameObjectPool Pooling;

        [field: SerializeField]
        private BalanceReference Balance;

        [field: SerializeField]
        private PhysicsReference Physics;

        [field: SerializeField]
        private ControllerInputContainer ControllerInputs;

        [field: SerializeField]
        private NgoNetSettings NetSettings;

        public override void InstallBindings()
        {
            Container.Bind<EventBus>().AsSingle().NonLazy();
            Container.BindInstance(ControllerInputs).AsSingle().NonLazy();
            Container.BindInstance(TickableManager).AsSingle().NonLazy();
            Container.BindInstance(Balance).AsSingle().NonLazy();
            Container.BindInstance(Physics).AsSingle().NonLazy();
            Container.BindInstance(Pooling).AsSingle().NonLazy();
            Container.BindInstance(NetSettings).AsSingle().NonLazy();

            NetworkInstaller.Install(Container);

            ControllerInputs.QueueForInject(Container);
        }
    }
}
