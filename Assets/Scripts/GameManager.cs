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
        public float LevelStartDelay = 1f;
        public float TurnDelay = 0.1f;
        public int PlayerFoodPoints = 200;
        public static GameManager Instance;
        public Player Player;
        public bool PlayersTurn = true;
                                                                                                                                                       
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
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
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
            Instance._level++;
            Instance.InitGame();
        }

        void InitGame()
        {
            _doingSetup = true;

            Instantiate(Player, new Vector3(0, 0, 0f), Quaternion.identity);

            _levelImage = GameObject.Find("LevelImage");
            _levelText = GameObject.Find("LevelText").GetComponent<Text>();
            _foodText = GameObject.Find("FoodText").GetComponent<Text>();
            _levelText.text = "Day " + _level;
            _levelImage.SetActive(true);
            Player.Food = PlayerFoodPoints;
            _foodText.text = $"Food {Player.Food}";
            Invoke("HideLevelImage", LevelStartDelay);
            _enemies.Clear();
            _boardScript.SetupScene(_level);
        }

        public void Restart()
        {
            StopAllCoroutines();
            _enemiesMoving = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
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
            if (PlayersTurn || _enemiesMoving || _doingSetup)
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

            yield return new WaitForSeconds(TurnDelay);
            _foodText.text = $"Food {Player.Food}";

            if (_enemies.Count == 0)
            {
                yield return new WaitForSeconds(TurnDelay);
            }

            foreach (var enemy in _enemies)
            {
                enemy.MoveEnemy();
                yield return new WaitForSeconds(enemy.MoveTime);
            }

            PlayersTurn = true;
            _enemiesMoving = false;
        }
    }
}
