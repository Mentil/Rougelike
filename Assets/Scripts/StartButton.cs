
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class StartButton : MonoBehaviour
    {
        void OnMouseDown()
        {
            SceneManager.LoadScene("Main");
        }

        public void OnClick()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
