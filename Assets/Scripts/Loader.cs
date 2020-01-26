
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts
{
    public class Loader : MonoBehaviour
    {
        public GameManager gameManager;
        public SoundManager soundManager;


        [UsedImplicitly]
        void Awake()
        {
            if (GameManager.instance == null)
                Instantiate(gameManager);

            if (SoundManager.instance == null)
                Instantiate(soundManager);
        }

        void Update()
        {
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }
    }
}