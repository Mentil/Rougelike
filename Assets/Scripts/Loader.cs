
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts
{
    public class Loader : MonoBehaviour
    {
        public GameManager gameManager;         //GameManager prefab to instantiate.
        public SoundManager soundManager;         //SoundManager prefab to instantiate.


        [UsedImplicitly]
        void Awake()
        {
            if (GameManager.instance == null)
                Instantiate(gameManager);

            if (SoundManager.instance == null)
                Instantiate(soundManager);
        }
    }
}