
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rougelike.Assets.Scripts
{
    public class MainMenuManager : MonoBehaviour
    {

        public void StartButton_OnClick()
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }

        public void OptionsButton_OnClick()
        {
            //SceneManager.LoadScene("Main");
        }

        public void ExitButton_OnClick()
        {
            Application.Quit();
        }
    }
}
