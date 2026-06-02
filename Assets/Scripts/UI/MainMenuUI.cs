using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Fireball.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Main Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quizButton;

        [Header("Panels")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject quizPanel;

        [Header("Scene Configuration")]
        [SerializeField] private string loadingSceneName = "LoadingScene";

        private void Start()
        {
            if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
            if (settingsButton != null) settingsButton.onClick.AddListener(ToggleSettings);
            if (quizButton != null) quizButton.onClick.AddListener(ToggleQuiz);

            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (quizPanel != null) quizPanel.SetActive(false);
        }

        private void OnPlayClicked()
        {
            // We'll pass the Level1 name to the LoadingScreenManager via a static or persistent object later.
            // For now, let's just load the LoadingScene.
            SceneManager.LoadScene(loadingSceneName);
        }

        private void ToggleSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(!settingsPanel.activeSelf);
        }

        private void ToggleQuiz()
        {
            if (quizPanel != null)
                quizPanel.SetActive(!quizPanel.activeSelf);
        }
    }
}
