
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rougelike.Assets.Scripts
{
    public class MainMenuManager : MonoBehaviour
    {
        [UsedImplicitly]
        public void StartButton_OnClick()
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }

        [UsedImplicitly]
        public void OptionsButton_OnClick()
        {
            //SceneManager.LoadScene("Main");
        }

        [UsedImplicitly]
        public void ExitButton_OnClick()
        {
            Application.Quit();
        }
    }
}
