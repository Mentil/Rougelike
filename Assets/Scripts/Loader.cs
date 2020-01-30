
using JetBrains.Annotations;
using UnityEngine;

namespace Rougelike.Assets.Scripts
{
    public class Loader : MonoBehaviour
    {
        public GameManager GameManager;
        public SoundManager SoundManager;


        [UsedImplicitly]
        void Awake()
        {
            if (GameManager.Instance == null)
                Instantiate(GameManager);

            if (SoundManager.Instance == null)
                Instantiate(SoundManager);
        }

        [UsedImplicitly]
        void Update()
        {
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }
    }
}