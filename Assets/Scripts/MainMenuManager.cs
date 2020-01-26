
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class MainMenuManager : MonoBehaviour
    {

        public void StartButton_OnClick()
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }

        public void OptionsButton_OnClick()
        {
            SceneManager.LoadScene("Options", LoadSceneMode.Single);
        }

        public void ExitButton_OnClick()
        {
            Application.Quit();
        }
    }
}
