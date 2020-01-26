
using UnityEngine;

namespace Assets.Scripts
{
    public class Enemy : MovingObject
    {
        public int playerDamage;
        public AudioClip[] attackSounds;

        private Animator _animator;
        private Transform _target;
        private bool _skipMove;

        protected override void Start()
        {
            GameManager.instance.AddEnemyToList(this);

            _animator = GetComponent<Animator>();

            _target = GameObject.FindGameObjectWithTag("Player").transform;

            base.Start();
        }


        protected override void AttemptMove<T>(int xDir, int yDir)
        {
            if (_skipMove)
            {
                _skipMove = false;
                return;

            }

            base.AttemptMove<T>(xDir, yDir);

            _skipMove = true;
        }

        public void MoveEnemy()
        {
            //Declare variables for X and Y axis move directions, these range from -1 to 1.
            //These values allow us to choose between the cardinal directions: up, down, left and right.
            int xDir = 0;
            int yDir = 0;

            //If the difference in positions is approximately zero (Epsilon) do the following:
            if (Mathf.Abs(_target.position.x - transform.position.x) < float.Epsilon)

                //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
                yDir = _target.position.y > transform.position.y ? 1 : -1;

            //If the difference in positions is not approximately zero (Epsilon) do the following:
            else
                //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
                xDir = _target.position.x > transform.position.x ? 1 : -1;

            //Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
            AttemptMove<Player>(xDir, yDir);
        }


        protected override void OnCantMove<T>(T component)
        {
            if (component is Player player)
            {
                player.LoseFood(playerDamage);
                _animator.SetTrigger("EnemyAttack");
                SoundManager.instance.RandomizeSfx(attackSounds);
            }
        }
    }
}
