using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Fireball.UI
{
    public class LoadingScreenManager : MonoBehaviour
    {
        private ProgressBar _progressBar;
        private Label _tipLabel;

        [Header("Settings")]
        [SerializeField] private string targetSceneName = "Level1";
        [SerializeField] private float minLoadingTime = 2f;
        [SerializeField] private string[] gameplayTips = {
            "Champagne foam pushes enemies back. Use it for crowd control!",
            "Moonshine Molotovs create fire zones. Don't stand in them!",
            "When both bottles are empty, you'll sprint faster. Run!",
            "Skeletons hate being wet. Well, not really, but it helps.",
            "Rival adventurers form shield walls. Flank them!",
            "The Shop is your friend. Spend gold wisely."
        };

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            _progressBar = root.Q<ProgressBar>("ProgressBar");
            _tipLabel = root.Q<Label>("TipLabel");

            StartCoroutine(LoadSceneAsync());
            StartCoroutine(CycleTips());
        }

        private IEnumerator LoadSceneAsync()
        {
            float startTime = Time.time;
            AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f) * 100f; // UI Toolkit ProgressBar is usually 0-100
                if (_progressBar != null) _progressBar.value = progress;

                if (operation.progress >= 0.9f && (Time.time - startTime) >= minLoadingTime)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }

        private IEnumerator CycleTips()
        {
            if (_tipLabel == null || gameplayTips.Length == 0) yield break;

            while (true)
            {
                _tipLabel.text = gameplayTips[Random.Range(0, gameplayTips.Length)];
                yield return new WaitForSeconds(4f);
            }
        }
    }
}
