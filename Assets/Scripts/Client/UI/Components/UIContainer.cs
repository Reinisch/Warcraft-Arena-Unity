using System.Collections.Generic;

namespace Client
{
    public class UIContainer : UIBehaviour
    {
        protected readonly List<UIBehaviour> Behaviours = new List<UIBehaviour>();

        public override void Initialize()
        {
            base.Initialize();

            Behaviours.ForEach(behaviour => behaviour.Initialize());
        }

        public override void Deinitialize()
        {
            Behaviours.ForEach(behaviour => behaviour.Deinitialize());

            base.Deinitialize();
        }

        public override void DoUpdate(int deltaTime)
        {
            base.DoUpdate(deltaTime);

            Behaviours.ForEach(behaviour => behaviour.DoUpdate(deltaTime));
        }
    }
}
