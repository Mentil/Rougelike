using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rougelike.Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public float levelStartDelay = 1f;
        public float turnDelay = 0.1f;
        public int playerFoodPoints = 200;
        public static GameManager instance;
        public Player player;
        public bool playersTurn = true;
                                                                                                                                                       
        private Text _levelText;
        private Text _foodText;
        private GameObject _levelImage;
        private BoardManager _boardScript;
        private int _level;
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

        void InitGame()
        {
            _doingSetup = true;

            Instantiate(player, new Vector3(0, 0, 0f), Quaternion.identity);

            _levelImage = GameObject.Find("LevelImage");
            _levelText = GameObject.Find("LevelText").GetComponent<Text>();
            _foodText = GameObject.Find("FoodText").GetComponent<Text>();
            _levelText.text = "Day " + _level;
            _levelImage.SetActive(true);
            player.food = playerFoodPoints;
            _foodText.text = $"Food {player.food}";
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
            //Set _levelText to display number of levels passed and game over message
            _levelText.text = "After " + _level + " days, you starved.";

            //Enable black background image gameObject.
            _levelImage.SetActive(true);

            //Disable this GameManager.
            enabled = false;
            Invoke("loadMenu", 2);

            Application.Quit();
        }

        IEnumerator MoveEnemies()
        {
            _enemiesMoving = true;

            yield return new WaitForSeconds(turnDelay);
            _foodText.text = $"Food {player.food}";

            if (_enemies.Count == 0)
            {
                yield return new WaitForSeconds(turnDelay);
            }

            foreach (var enemy in _enemies)
            {
                enemy.MoveEnemy();
                yield return new WaitForSeconds(enemy.moveTime);
            }

            playersTurn = true;
            _enemiesMoving = false;
        }
    }
}
