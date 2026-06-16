using Client;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Workflow
{
    internal class ClientInstaller : MonoInstaller
    {
        [field: SerializeField]
        private InterfaceReference Interface;

        [field: SerializeField]
        private InputReference Input;

        [field: SerializeField]
        private GameOptionsReference Options;

        [field: SerializeField]
        private LocalizationReference Localization;

        [field: SerializeField]
        private RenderingReference Rendering;

        [field: SerializeField]
        private EffectReference Effects;

        [field: SerializeField]
        private SoundReference Sound;

        [field: SerializeField]
        private TargetingReference Targeting;

        [field: SerializeField]
        private TargetingSpellReference TargetingSpell;

        [field: SerializeField]
        private CameraReference Camera;

        [field: SerializeField]
        private SpellOverlayReference SpellOverlay;

        [field: SerializeField]
        private TooltipReference Tooltips;

        [field: SerializeField]
        private ProjectileModule ProjectileModule;

        public override void InstallBindings()
        {
            Container.BindInstance(Interface).AsSingle().NonLazy();
            Container.BindInstance(Input).AsSingle().NonLazy();
            Container.BindInstance(Options).AsSingle().NonLazy();
            Container.BindInstance(Localization).AsSingle().NonLazy();
            Container.BindInstance(Rendering).AsSingle().NonLazy();
            Container.BindInstance(Effects).AsSingle().NonLazy();
            Container.BindInstance(Targeting).AsSingle().NonLazy();
            Container.BindInstance(TargetingSpell).AsSingle().NonLazy();
            Container.BindInstance(Camera).AsSingle().NonLazy();
            Container.BindInstance(SpellOverlay).AsSingle().NonLazy();
            Container.BindInstance(Tooltips).AsSingle().NonLazy();
            Container.BindInstance(ProjectileModule).AsSingle().NonLazy();
            Container.BindInstance(Sound).AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<ProjectileTracker.Factory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ProjectileTracker.MemoryPool>().AsSingle().WithArguments(10).NonLazy();
            Container.BindInterfacesAndSelfTo<ProjectileLaunchTracker.Factory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ProjectileLaunchTracker.MemoryPool>().AsSingle().WithArguments(10).NonLazy();

            Container.Bind<UnitFramePresenter>().AsTransient();
            Container.Bind<BattleHudPresenter>().AsTransient();
            Container.Bind<ComboFramePresenter>().AsTransient();
            Container.Bind<CastFramePresenter>().AsTransient();
            Container.Bind<BuffDisplayPresenter>().AsTransient();
            Container.Bind<ActionErrorDisplayPresenter>().AsTransient();
            Container.Bind<LobbyPresenter>().AsTransient();
        }
    }
}