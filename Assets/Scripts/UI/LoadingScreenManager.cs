using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

namespace Fireball.UI
{
    public class LoadingScreenManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI tipText;
        [SerializeField] private Image background;

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

        private void Start()
        {
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
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                if (progressBar != null) progressBar.value = progress;

                // Check if loading is complete and minimum time has passed
                if (operation.progress >= 0.9f && (Time.time - startTime) >= minLoadingTime)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }

        private IEnumerator CycleTips()
        {
            if (tipText == null || gameplayTips.Length == 0) yield break;

            while (true)
            {
                tipText.text = gameplayTips[Random.Range(0, gameplayTips.Length)];
                yield return new WaitForSeconds(4f);
            }
        }
    }
}
