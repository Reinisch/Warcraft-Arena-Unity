using System.Collections.Generic;

namespace Client
{
    public class UIContainer : UIBehaviour
    {
        protected readonly List<UIBehaviour> behaviours = new List<UIBehaviour>();

        public override void Initialize()
        {
            base.Initialize();

            behaviours.ForEach(behaviour => behaviour.Initialize());
        }

        public override void Deinitialize()
        {
            behaviours.ForEach(behaviour => behaviour.Deinitialize());

            base.Deinitialize();
        }

        public override void DoUpdate(int deltaTime)
        {
            base.DoUpdate(deltaTime);

            behaviours.ForEach(behaviour => behaviour.DoUpdate(deltaTime));
        }
    }
}
