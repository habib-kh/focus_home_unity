using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace FocusHome
{
    public class NavigationUI : MonoBehaviour
    {
        [Header("Navigation Buttons")]
        [SerializeField] private Button timerButton;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button profileButton;

        [Header("Button Highlights")]
        [SerializeField] private Image timerHighlight;
        [SerializeField] private Image homeHighlight;
        [SerializeField] private Image shopHighlight;
        [SerializeField] private Image profileHighlight;

        [Header("Scene Names")]
        [SerializeField] private string timerSceneName = "TimerScene";
        [SerializeField] private string homeSceneName = "HomeScene";
        [SerializeField] private string shopSceneName = "ShopScene";
        [SerializeField] private string profileSceneName = "ProfileScene";

        private void Start()
        {
            SetupButtons();
            UpdateHighlights();
        }

        private void SetupButtons()
        {
            timerButton?.onClick.AddListener(() => LoadScene(timerSceneName));
            homeButton?.onClick.AddListener(() => LoadScene(homeSceneName));
            shopButton?.onClick.AddListener(() => LoadScene(shopSceneName));
            profileButton?.onClick.AddListener(() => LoadScene(profileSceneName));
        }

        private void LoadScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        private void UpdateHighlights()
        {
            string currentScene = SceneManager.GetActiveScene().name;

            SetHighlight(timerHighlight, currentScene == timerSceneName);
            SetHighlight(homeHighlight, currentScene == homeSceneName);
            SetHighlight(shopHighlight, currentScene == shopSceneName);
            SetHighlight(profileHighlight, currentScene == profileSceneName);
        }

        private void SetHighlight(Image highlight, bool active)
        {
            if (highlight != null)
            {
                highlight.gameObject.SetActive(active);
            }
        }
    }
}
