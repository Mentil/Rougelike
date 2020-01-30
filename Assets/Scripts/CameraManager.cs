
using JetBrains.Annotations;
using UnityEngine;

namespace Rougelike.Assets.Scripts
{
    public class CameraManager : MonoBehaviour
    {
        [UsedImplicitly]
        void Update()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -1);
            }

        }
}


}
