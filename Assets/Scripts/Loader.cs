
using UnityEngine;

namespace Assets.Scripts
{
    public class Loader : MonoBehaviour
    {
        public GameObject gameManager;

        // Start is called before the first frame update
        void Awake()
        {
            if (GameManager.instance == null)
            {
                Instantiate(gameManager);
            }
        }
    }
}
