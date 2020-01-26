
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{

    public class GameManager : MonoBehaviour
    {
        public float levelStartDelay = 1f;
        public float turnDelay = 0.1f;
        public int playerFoodPoints = 200;
        public static GameManager instance;
        [HideInInspector] public bool playersTurn = true;

        private Text _levelText;
        private GameObject _levelImage;
        private BoardManager _boardScript;
        private int _level = 1;
        private List<Enemy> _enemies;
        private bool _enemiesMoving;
        private bool _doingSetup = true;

        [UsedImplicitly]
        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            _enemies = new List<Enemy>();
            _boardScript = GetComponent<BoardManager>();

            InitGame();
        }

        //this is called only once, and the parameter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [UsedImplicitly]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void CallbackInitialization()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            instance._level++;
            instance.InitGame();
        }


        //Initializes the game for each level.
        void InitGame()
        {
            _doingSetup = true;

            _levelImage = GameObject.Find("LevelImage");
            _levelText = GameObject.Find("LevelText").GetComponent<Text>();

            _levelText.text = "Day " + _level;
            _levelImage.SetActive(true);

            Invoke("HideLevelImage", levelStartDelay);

            _enemies.Clear();
            _boardScript.SetupScene(_level);
        }

        [UsedImplicitly]
        void HideLevelImage()
        {
            _levelImage.SetActive(false);
            _doingSetup = false;
        }

        [UsedImplicitly]
        void Update()
        {
            if (playersTurn || _enemiesMoving || _doingSetup)
                return;
            StartCoroutine(MoveEnemies());
        }

        public void AddEnemyToList(Enemy script)
        {
            _enemies.Add(script);
        }

        public void GameOver()
        {
            _levelText.text = "After " + _level + " days, you starved.";
            _levelImage.SetActive(true);

            enabled = false;
            Invoke("LoadMainMenu", levelStartDelay);
        }

        void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        //Co routine to move enemies in sequence.
        IEnumerator MoveEnemies()
        {
            _enemiesMoving = true;
            yield return new WaitForSeconds(turnDelay);

            //If there are no enemies spawned (IE in first level):
            if (_enemies.Count == 0)
            {
                yield return new WaitForSeconds(turnDelay);
            }

            foreach (var enemy in _enemies)
            {
                enemy.MoveEnemy();
                yield return new WaitForSeconds(turnDelay);
            }
            playersTurn = true;
            _enemiesMoving = false;
            yield return new WaitForSeconds(turnDelay);
        }
    }
}

