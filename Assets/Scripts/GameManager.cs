
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
        public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
        public float turnDelay = 0.0f;                          //Delay between each Player turn.
        public int playerFoodPoints = 100;                      //Starting value for Player food points.
        public static GameManager instance;                     //Static instance of GameManager which allows it to be accessed by any other script.
        [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.


        private Text _levelText;                                 //Text to display current level number.
        private GameObject _levelImage;                          //Image to block out level as levels are being set up, background for _levelText.
        private BoardManager _boardScript;                       //Store a reference to our BoardManager which will set up the level.
        private int _level = 1;                                  //Current level number, expressed in game as "Day 1".
        private List<Enemy> _enemies;                            //List of all Enemy units, used to issue them move commands.
        private bool _enemiesMoving;                             //Boolean to check if enemies are moving.
        private bool _doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.



        //Awake is always called before any Start functions
        [UsedImplicitly]
        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            _enemies = new List<Enemy>();

            //Get a component reference to the attached BoardManager script
            _boardScript = GetComponent<BoardManager>();

            InitGame();
        }

        //this is called only once, and the parameter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [UsedImplicitly]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void CallbackInitialization()
        {
            //register the callback to be called every time the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            instance._level++;
            instance.InitGame();
        }


        //Initializes the game for each level.
        void InitGame()
        {
            //While doingSetup is true the player can't move, prevent player from moving while title card is up.
            _doingSetup = true;

            //Get a reference to our image LevelImage by finding it by name.
            _levelImage = GameObject.Find("LevelImage");

            //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
            _levelText = GameObject.Find("LevelText").GetComponent<Text>();


            //Set the text of _levelText to the string "Day" and append the current level number.
            _levelText.text = "Day " + _level;

            //Set levelImage to active blocking player's view of the game board during setup.
            _levelImage.SetActive(true);

            //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
            Invoke("HideLevelImage", levelStartDelay);

            //Clear any Enemy objects in our List to prepare for next level.
            _enemies.Clear();

            //Call the SetupScene function of the BoardManager script, pass it current level number.
            _boardScript.SetupScene(_level);

        }


        //Hides black image used between levels
        [UsedImplicitly]
        void HideLevelImage()
        {
            //Disable the levelImage gameObject.
            _levelImage.SetActive(false);

            //Set doingSetup to false allowing player to move again.
            _doingSetup = false;
        }

        //Update is called every frame.
        [UsedImplicitly]
        void Update()
        {
            //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
            if (playersTurn || _enemiesMoving || _doingSetup)

                //If any of these are true, return and do not start MoveEnemies.
                return;

            //Start moving enemies.
            StartCoroutine(MoveEnemies());
        }

        //Call this to add the passed in Enemy to the List of Enemy objects.
        public void AddEnemyToList(Enemy script)
        {
            //Add Enemy to List enemies.
            _enemies.Add(script);
        }


        //GameOver is called when the player reaches 0 food points
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

        void loadMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        //Co routine to move enemies in sequence.
        IEnumerator MoveEnemies()
        {
            //While enemiesMoving is true player is unable to move.
            _enemiesMoving = true;

            //Wait for turnDelay seconds, defaults to .1 (100 ms).
            yield return new WaitForSeconds(turnDelay);

            //If there are no enemies spawned (IE in first level):
            if (_enemies.Count == 0)
            {
                //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
                //yield return new WaitForSeconds(turnDelay);
            }

            //Loop through List of Enemy objects.
            foreach (var enemy in _enemies)
            {
                enemy.MoveEnemy();

                //Wait for Enemy's moveTime before moving next Enemy, 
                //yield return new WaitForSeconds(enemy.moveTime);
            }
            //Once Enemies are done moving, set playersTurn to true so player can move.
            playersTurn = true;

            //Enemies are done moving, set enemiesMoving to false.
            _enemiesMoving = false;
        }
    }
}

