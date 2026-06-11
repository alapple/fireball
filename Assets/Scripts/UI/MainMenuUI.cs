using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace Fireball.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Fireball.ScriptableObjects.PlayerStats playerStats;
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

            // Find close buttons in panels
            root.Q<Button>("CloseSettings")?.RegisterCallback<ClickEvent>(evt => ToggleSettings());
            root.Q<Button>("CloseQuiz")?.RegisterCallback<ClickEvent>(evt => ToggleQuiz());

            // Initial State
            if (_settingsPanel != null) _settingsPanel.style.display = DisplayStyle.None;
            if (_quizPanel != null) _quizPanel.style.display = DisplayStyle.None;

            // Setup Cursor
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            // Hover Effects
            RegisterButtonEffects(_playButton, new Color(1f, 0.4f, 0f), new Color(0.7f, 0.2f, 0f));
            RegisterButtonEffects(_settingsButton, new Color(0.3f, 0.3f, 0.3f), new Color(0.2f, 0.2f, 0.2f));
            RegisterButtonEffects(_quizButton, new Color(0.3f, 0.3f, 0.3f), new Color(0.2f, 0.2f, 0.2f));
        }

        private void RegisterButtonEffects(Button button, Color normalColor, Color hoverColor)
        {
            if (button == null) return;
            button.RegisterCallback<MouseEnterEvent>(evt => button.style.backgroundColor = hoverColor);
            button.RegisterCallback<MouseLeaveEvent>(evt => button.style.backgroundColor = normalColor);
        }

        private void OnPlayClicked()
        {
            if (playerStats != null) playerStats.ResetStats();
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
