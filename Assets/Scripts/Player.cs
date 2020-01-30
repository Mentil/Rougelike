
using JetBrains.Annotations;
using UnityEngine;

namespace Rougelike.Assets.Scripts
{
    public class Player : MovingObject
    {
        public AudioClip[] MoveSounds;
        public AudioClip[] EatSounds;
        public AudioClip[] DrinkSounds;
        public AudioClip GameOverSound;
        public int Food;

        private const int PointsPerFood = 10;
        private const int PointsPerSoda = 20;
        private const int WallDamage = 1;
        private Animator _animator;         
        private bool _isColliding;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 _touchOrigin = -Vector2.one;
#endif

        protected override void Start()
        {
            _animator = GetComponent<Animator>();
            Food = GameManager.Instance.PlayerFoodPoints;

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
            if (!GameManager.Instance.PlayersTurn) return;

#if UNITY_STANDALONE || UNITY_WEBGL

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
            Food--;

            base.AttemptMove<T>(xDir, yDir);

            if (Move(xDir, yDir, out _))
            {
                SoundManager.Instance.RandomizeSfx(MoveSounds);
            }

            CheckIfGameOver();

            GameManager.Instance.PlayersTurn = false;
        }

        protected override void OnCantMove<T>(T component)
        {
            if (component is Wall wall)
            {
                wall.DamageWall(WallDamage);
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
                GameManager.Instance.PlayerFoodPoints = Food;
                Invoke("Restart", GameManager.Instance.LevelStartDelay);
                enabled = false;
            }
            else if (other.tag == "Food")
            {
                Food += PointsPerFood;
                SoundManager.Instance.RandomizeSfx(EatSounds);
                other.gameObject.SetActive(false);
            }
            else if (other.tag == "Soda")
            {
                Food += PointsPerSoda;
                SoundManager.Instance.RandomizeSfx(DrinkSounds);
                other.gameObject.SetActive(false);
            }
        }

        [UsedImplicitly]
        private void Restart()
        {
            GameManager.Instance.Restart();
        }

        public void LoseFood(int loss)
        {
            _animator.SetTrigger("PlayerHit");
            Food -= loss;
            CheckIfGameOver();
        }

        private void CheckIfGameOver()
        {
            if (Food <= 0)
            {
                SoundManager.Instance.PlaySingle(GameOverSound);
                SoundManager.Instance.MusicSource.Stop();
                GameManager.Instance.GameOver();
            }
        }
    }
}
