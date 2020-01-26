
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Player : MovingObject
    {
        public float restartLevelDelay = 1f;
        public int pointsPerFood = 10;
        public int pointsPerSoda = 20;
        public int wallDamage = 1;
        public Text foodText;
        public AudioClip[] moveSounds;
        public AudioClip[] eatSounds;
        public AudioClip[] drinkSounds;
        public AudioClip gameOverSound;

        private Animator _animator;         
        private int _food;                  
        private bool _isColliding;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 _touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif

        protected override void Start()
        {
            _animator = GetComponent<Animator>();
            _food = GameManager.instance.playerFoodPoints;
            foodText.text = "Food: " + _food;

            base.Start();
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            //GameManager.instance.playerFoodPoints = _food;
        }


        [UsedImplicitly]
        private void Update()
        {
            if (!GameManager.instance.playersTurn) return;

#if UNITY_STANDALONE || UNITY_WEBPLAYER

            var horizontal = (int)(Input.GetAxisRaw("Horizontal"));
            var vertical = (int)(Input.GetAxisRaw("Vertical"));

            if (horizontal != 0)
            {
                vertical = 0;
            }

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			if (Input.touchCount > 0)
			{
				Touch myTouch = Input.touches[0];
				
				if (myTouch.phase == TouchPhase.Began)
				{
					_touchOrigin = myTouch.position;
				}
				
				else if (myTouch.phase == TouchPhase.Ended && _touchOrigin.x >= 0)
				{
					Vector2 touchEnd = myTouch.position;
					
					float x = touchEnd.x - _touchOrigin.x;
					
					float y = touchEnd.y - _touchOrigin.y;
					
					_touchOrigin.x = -1;
					
					if (Mathf.Abs(x) > Mathf.Abs(y))
						horizontal = x > 0 ? 1 : -1;
					else
						vertical = y > 0 ? 1 : -1;
				}
			}
#endif
            if (horizontal != 0 || vertical != 0)
            {
                AttemptMove<Wall>(horizontal, vertical);
            }
        }

        protected override void AttemptMove<T>(int xDir, int yDir)
        {
            _food--;
            foodText.text = "Food: " + _food;

            base.AttemptMove<T>(xDir, yDir);

            RaycastHit2D hit;

            if (Move(xDir, yDir, out hit))
            {
                SoundManager.instance.RandomizeSfx(moveSounds);
            }

            CheckIfGameOver();

            GameManager.instance.playersTurn = false;
        }

        protected override void OnCantMove<T>(T component)
        {
            if (component is Wall wall)
            {
                wall.DamageWall(wallDamage);
                _animator.SetTrigger("PlayerChop");
            }
        }

        [UsedImplicitly]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(_isColliding)
                return;

            if (other.tag == "Exit")
            {
                _isColliding = true;
                GameManager.instance.playerFoodPoints = _food;
                Invoke("Restart", restartLevelDelay);
                enabled = false;
            }
            else if (other.tag == "Food")
            {
                _food += pointsPerFood;
                foodText.text = "+" + pointsPerFood + " Food: " + _food;
                SoundManager.instance.RandomizeSfx(eatSounds);
                other.gameObject.SetActive(false);
            }
            else if (other.tag == "Soda")
            {
                _food += pointsPerSoda;
                foodText.text = "+" + pointsPerSoda + " Food: " + _food;
                SoundManager.instance.RandomizeSfx(drinkSounds);
                other.gameObject.SetActive(false);
            }
        }

        [UsedImplicitly]
        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        public void LoseFood(int loss)
        {
            _animator.SetTrigger("PlayerHit");
            _food -= loss;
            foodText.text = "-" + loss + " Food: " + _food;
            CheckIfGameOver();
        }

        private void CheckIfGameOver()
        {
            if (_food <= 0)
            {
                SoundManager.instance.PlaySingle(gameOverSound);
                SoundManager.instance.musicSource.Stop();
                GameManager.instance.GameOver();
            }
        }
    }
}
