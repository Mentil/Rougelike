
using JetBrains.Annotations;
using UnityEngine;

namespace Rougelike.Assets.Scripts
{
    public class Wall : MonoBehaviour
    {
        public AudioClip[] ChopSounds;
        public Sprite DmgSprite;
        public int Hp = 3;

        private SpriteRenderer _spriteRenderer;


        [UsedImplicitly]
        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void DamageWall(int loss)
        {
            SoundManager.Instance.RandomizeSfx(ChopSounds);

            _spriteRenderer.sprite = DmgSprite;

            Hp -= loss;

            if (Hp <= 0)
                gameObject.SetActive(false);
        }
    }
}