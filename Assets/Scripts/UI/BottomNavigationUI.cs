using UnityEngine;
using UnityEngine.UI;

namespace FocusHome
{
    public class BottomNavigationUI : MonoBehaviour
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

        [Header("Highlight Colors")]
        [SerializeField] private Color activeColor = new Color(0.2f, 0.6f, 0.9f);
        [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        private void Start()
        {
            SetupButtons();
            UpdateHighlights();
        }

        private void SetupButtons()
        {
            if (timerButton != null)
                timerButton.onClick.AddListener(() => Navigate(SceneLoader.TIMER_SCENE));

            if (homeButton != null)
                homeButton.onClick.AddListener(() => Navigate(SceneLoader.HOME_SCENE));

            if (shopButton != null)
                shopButton.onClick.AddListener(() => Navigate(SceneLoader.SHOP_SCENE));

            if (profileButton != null)
                profileButton.onClick.AddListener(() => Navigate(SceneLoader.PROFILE_SCENE));
        }

        private void Navigate(string sceneName)
        {
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadScene(sceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
        }

        private void UpdateHighlights()
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            SetHighlight(timerHighlight, currentScene == SceneLoader.TIMER_SCENE);
            SetHighlight(homeHighlight, currentScene == SceneLoader.HOME_SCENE);
            SetHighlight(shopHighlight, currentScene == SceneLoader.SHOP_SCENE);
            SetHighlight(profileHighlight, currentScene == SceneLoader.PROFILE_SCENE);
        }

        private void SetHighlight(Image highlight, bool active)
        {
            if (highlight != null)
            {
                highlight.color = active ? activeColor : inactiveColor;
            }
        }
    }
}
