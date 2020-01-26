
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Player : MovingObject
    {
        public int pointsPerFood = 10;              
        public int pointsPerSoda = 20;              
        public int wallDamage = 1;                  
        public Text foodText;                       
        public AudioClip[] moveSounds;                
        public AudioClip[] eatSounds;                 
        public AudioClip[] drinkSounds;               
        public AudioClip gameOverSound;               

        private Animator _animator;                  //Used to store a reference to the Player's animator component.
        private int _food;

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
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set _touchOrigin to the position of that touch
					_touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of _touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && _touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - _touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - _touchOrigin.y;
					
					//Set _touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					_touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif
            if (horizontal != 0 || vertical != 0)
            {
                AttemptMove<Wall>(horizontal, vertical);
            }
        }

        protected override bool AttemptMove<T>(int xDir, int yDir)
        {
            GameManager.instance.playersTurn = true;
            _food--;

            foodText.text = "Food: " + _food;

            //If Move returns true, meaning Player was able to move into an empty space.
            if (base.AttemptMove<T>(xDir, yDir))
            {
                SoundManager.instance.RandomizeSfx(moveSounds);
                return true;
            }

            CheckIfGameOver();

            GameManager.instance.playersTurn = false;
            return false;
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
            if (other.tag == "Exit")
            {
                GameManager.instance.playerFoodPoints = _food;
                enabled = false;
                Invoke("Restart", GameManager.instance.levelStartDelay);
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

