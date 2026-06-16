using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Common
{
    public class GameObjectPool : ScriptableReference
    {
        private static GameObjectPool Instance;

        [SerializeField, UsedImplicitly]
        private string containerTag;

        [Inject]
        private GameObjectFactory objectFactory;

        private readonly Dictionary<EntityId, Stack<GameObject>> pooledGameObjectsByProtoId = new();
        private readonly Dictionary<GameObject, EntityId> takenObjectProtoIds = new();
        private Transform container;

        protected override void OnRegistered()
        {
            container = GameObject.FindGameObjectWithTag(containerTag).transform;

            Instance = this;
        }

        protected override void OnUnregister()
        {
            foreach (var pooledObjects in pooledGameObjectsByProtoId)
            foreach (var pooledObject in pooledObjects.Value)
                Destroy(pooledObject);

            pooledGameObjectsByProtoId.Clear();
            takenObjectProtoIds.Clear();
            Instance = null;
        }

        private bool ProcessReturn(GameObject returnedObject, bool destroyed)
        {
            if (!takenObjectProtoIds.Remove(returnedObject, out EntityId protoId))
                return false;

            if (destroyed)
                return false;

            ProcessPooling(returnedObject, protoId);
            return true;
        }

        private void CreateAndPool(GameObject prefab, EntityId protoId)
        {
            GameObject pooledObject = objectFactory.Create(prefab, Vector3.zero, Quaternion.identity, container);
            pooledObject.SetActive(false);
            pooledObject.transform.SetParent(container);

            if (pooledGameObjectsByProtoId.TryGetValue(protoId, out Stack<GameObject> pooledObjects))
                pooledObjects.Push(pooledObject);
            else
            {
                pooledObjects = new Stack<GameObject>();
                pooledObjects.Push(pooledObject);
                pooledGameObjectsByProtoId.Add(protoId, pooledObjects);
            }
        }

        private void ProcessPooling(GameObject pooledObject, EntityId protoId)
        {
            pooledObject.SetActive(false);
            pooledObject.transform.SetParent(container);

            if (pooledGameObjectsByProtoId.TryGetValue(protoId, out Stack<GameObject> pooledObjects))
                pooledObjects.Push(pooledObject);
            else
            {
                pooledObjects = new Stack<GameObject>();
                pooledObjects.Push(pooledObject);
                pooledGameObjectsByProtoId.Add(protoId, pooledObjects);
            }
        }

        private T TakeOrCreate<T>(T prototype, Vector3 position, Quaternion rotation, Transform parent) where T : Behaviour
        {
            return TakeOrCreate(prototype.gameObject, position, rotation, parent).GetComponent<T>();
        }

        private GameObject TakeOrCreate(GameObject prototype, Vector3 position, Quaternion rotation, Transform parent)
        {
            EntityId protoId = prototype.GetEntityId();
            GameObject newObject = TakeIfAvailable(protoId, position, rotation, parent);
            if (newObject == null)
            {
                newObject = objectFactory.Create(prototype, position, rotation, parent);
                takenObjectProtoIds.Add(newObject, protoId);
            }

            return newObject;
        }
        
        private GameObject TakeIfAvailable(EntityId protoId, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (pooledGameObjectsByProtoId.TryGetValue(protoId, out Stack<GameObject> pooledObjects))
            {
                while (pooledObjects.Count > 0)
                {
                    GameObject objectToTake = pooledObjects.Pop();
                    if (objectToTake == null)
                        continue;

                    objectToTake.transform.position = position;
                    objectToTake.transform.rotation = rotation;
                    objectToTake.transform.SetParent(parent ?? container);
                    objectToTake.SetActive(true);

                    takenObjectProtoIds.Add(objectToTake, protoId);
                    return objectToTake;
                }
            }
            return null;
        }

        public static void PreInstantiate(GameObject prototype, int preinstantiatedCount)
        {
            if (Instance == null)
                return;

            EntityId protoId = prototype.GetEntityId();
            int existingCount = 0;

            if (Instance.pooledGameObjectsByProtoId.TryGetValue(protoId, out Stack<GameObject> pooledObjects))
                existingCount = pooledObjects.Count;

            for (int i = existingCount; i < preinstantiatedCount; i++)
                Instance.CreateAndPool(prototype, protoId);
        }

        public static void PreInstantiate<T>(T prototypeBehaviour, int preinstantiatedCount) where T: Behaviour
        {
            PreInstantiate(prototypeBehaviour.gameObject, preinstantiatedCount);
        }

        public static GameObject Take(GameObject prototype, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return Instance.TakeOrCreate(prototype, position, rotation, parent);
        }

        public static T Take<T>(T prototype, Vector3 position, Quaternion rotation, Transform parent = null) where T: Behaviour
        {
            return Instance.TakeOrCreate(prototype, position, rotation, parent);
        }

        public static T Take<T>(T prototype) where T : Behaviour
        {
            return Instance.TakeOrCreate(prototype, prototype.transform.localPosition, prototype.transform.rotation, null);
        }

        public static void Return(GameObject takenObject, bool destroyed)
        {
            bool returnedSuccessfully = Instance?.ProcessReturn(takenObject, destroyed) ?? false;
            if (!returnedSuccessfully && !destroyed)
                Destroy(takenObject);
        }

        public static void Return<T>(T takenObject, bool destroyed) where T: Behaviour
        {
            Return(takenObject.gameObject, destroyed);
        }
    }
}
