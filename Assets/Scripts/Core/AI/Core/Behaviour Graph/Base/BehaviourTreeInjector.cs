using UnityEngine;
using Zenject;

namespace Core.Scenario
{
    public class BehaviourTreeInjector : MonoBehaviour
    {
        [Inject]
        public DiContainer DiContainer { get; private set; }
    }
}