
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager instance = null;

        public BoardManager boardScript;

        private int level = 3;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }


            DontDestroyOnLoad(this);
            boardScript = GetComponent<BoardManager>();

            InitGame();
        }

        void InitGame()
        {
            boardScript.SetupScene(level);
        }
    }
}