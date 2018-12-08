using Core;
using UnityEngine;

namespace Client
{
    public class BattleScreen : MonoBehaviour
    {
        public void Initialize(PhotonBoltManager photonManager)
        {
            gameObject.SetActive(false);
        }

        public void Deinitialize()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
