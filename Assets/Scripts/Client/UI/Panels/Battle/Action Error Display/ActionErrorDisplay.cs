using Client.Sound;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Client
{
    public class ActionErrorDisplay : MonoBehaviour
    {
        [Inject] private ActionErrorDisplayPresenter presenter;

        [SerializeField, UsedImplicitly] private ActionErrorItem errorItemPrototype;
        [SerializeField, UsedImplicitly] private ActionErrorDisplaySettings settings;
        [SerializeField, UsedImplicitly] private SoundEntry errorAppearSound;
        [SerializeField, UsedImplicitly] private RectTransform errorContainer;
        [SerializeField, UsedImplicitly] private int preinstantiatedCount = 20;

        public bool AllowRepeating => settings.AllowRepeating;

        internal ActionErrorDisplayPresenter Presenter => presenter;

        [UsedImplicitly]
        private void Awake()
        {
            presenter.Initialize(this);
        }

        public void PreinstantiateItems()
        {
            GameObjectPool.PreInstantiate(errorItemPrototype.gameObject, preinstantiatedCount);
        }

        public void PlayAppearSound()
        {
            errorAppearSound?.Play(transform.position);
        }

        public ActionErrorItem SpawnError(SpellCastResult castResult)
        {
            ActionErrorItem newError = GameObjectPool.Take(errorItemPrototype, errorContainer.position, errorContainer.rotation, errorContainer);
            newError.SetErrorText(castResult);
            newError.RectTransform.SetAsFirstSibling();
            return newError;
        }

        public void ReturnError(ActionErrorItem item, bool destroyed)
        {
            GameObjectPool.Return(item, destroyed);

            if (destroyed)
                Destroy(item);
        }
    }
}
