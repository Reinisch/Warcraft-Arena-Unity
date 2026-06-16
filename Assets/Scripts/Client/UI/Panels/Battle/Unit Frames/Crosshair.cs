using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class Crosshair: MonoBehaviour
    {
        [SerializeField]
        private Image icon;

        public void SetActive(bool active) => icon.enabled = active;
    }
}