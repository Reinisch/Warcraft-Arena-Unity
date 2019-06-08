using System.Collections.Generic;
using Common;

namespace Client
{
    public class UIContainer : UIBehaviour
    {
        private readonly List<UIBehaviour> behaviours = new List<UIBehaviour>();

        public override void Initialize()
        {
            base.Initialize();

            Assert.IsTrue(behaviours.Count == 0, $"UI Container: {this.GetPath()} already has some registered behaviours while initializing!");
        }

        public override void Deinitialize()
        {
            Assert.IsTrue(behaviours.Count == 0, $"UI Container: {this.GetPath()} still has some registered behaviours while deinitializing!");

            base.Deinitialize();
        }

        public override void DoUpdate(int deltaTime)
        {
            base.DoUpdate(deltaTime);

            foreach (UIBehaviour behaviour in behaviours)
                behaviour.DoUpdate(deltaTime);
        }

        protected void RegisterBehaviour(UIBehaviour behaviour)
        {
            Assert.IsFalse(behaviours.Contains(behaviour), $"UI Container: {this.GetPath()} already contains: {behaviour.GetPath()}");

            behaviour.Initialize();

            behaviours.Add(behaviour);
        }

        protected void UnregisterBehaviour(UIBehaviour behaviour)
        {
            Assert.IsTrue(behaviours.Contains(behaviour), $"UI Container: {this.GetPath()} does not contain: {behaviour.GetPath()}");

            behaviour.Deinitialize();

            behaviours.Remove(behaviour);
        }
    }
}
