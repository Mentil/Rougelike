
using JetBrains.Annotations;
using UnityEngine;

namespace Rougelike.Assets.Scripts
{
    public class Wall : MonoBehaviour
    {
        public AudioClip[] chopSounds;
        public Sprite dmgSprite;
        public int hp = 3;


        private SpriteRenderer _spriteRenderer;


        [UsedImplicitly]
        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void DamageWall(int loss)
        {
            SoundManager.instance.RandomizeSfx(chopSounds);

            _spriteRenderer.sprite = dmgSprite;

            hp -= loss;

            if (hp <= 0)
                gameObject.SetActive(false);
        }
    }
}