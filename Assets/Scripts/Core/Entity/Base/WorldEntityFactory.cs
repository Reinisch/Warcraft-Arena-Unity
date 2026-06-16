using Assets.Scripts.Core;
using Zenject;

namespace Core
{
    public class WorldEntityFactory
    {
        [Inject]
        private DiContainer diContainer;

        private ulong id = 1;

        internal TEntity Create<TEntity>(WorldEntityPrefab prefab, Entity.CreateToken createToken) where TEntity : Unit
        {
            createToken.Id = id++;
            TEntity entity = diContainer.InstantiatePrefab(prefab.Prototype).GetComponent<TEntity>();
            entity.Attached(createToken);
           
            return entity;
        }
    }
}