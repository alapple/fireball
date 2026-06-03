using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace Fireball.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        private Button _playButton;
        private Button _settingsButton;
        private Button _quizButton;

        private VisualElement _settingsPanel;
        private VisualElement _quizPanel;

        [Header("Scene Configuration")]
        [SerializeField] private string loadingSceneName = "LoadingScene";

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _playButton = root.Q<Button>("PlayButton");
            _settingsButton = root.Q<Button>("SettingsButton");
            _quizButton = root.Q<Button>("QuizButton");

            _settingsPanel = root.Q<VisualElement>("SettingsPanel");
            _quizPanel = root.Q<VisualElement>("QuizPanel");

            if (_playButton != null) _playButton.clicked += OnPlayClicked;
            if (_settingsButton != null) _settingsButton.clicked += ToggleSettings;
            if (_quizButton != null) _quizButton.clicked += ToggleQuiz;

            if (_settingsPanel != null) _settingsPanel.style.display = DisplayStyle.None;
            if (_quizPanel != null) _quizPanel.style.display = DisplayStyle.None;
        }

        private void OnPlayClicked()
        {
            SceneManager.LoadScene(loadingSceneName);
        }

        private void ToggleSettings()
        {
            if (_settingsPanel != null)
            {
                bool isVisible = _settingsPanel.style.display == DisplayStyle.Flex;
                _settingsPanel.style.display = isVisible ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        private void ToggleQuiz()
        {
            if (_quizPanel != null)
            {
                bool isVisible = _quizPanel.style.display == DisplayStyle.Flex;
                _quizPanel.style.display = isVisible ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }
    }
}
