using UnityEngine;
using UnityEngine.SceneManagement;

namespace FocusHome
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        public const string TIMER_SCENE = "TimerScene";
        public const string HOME_SCENE = "HomeScene";
        public const string SHOP_SCENE = "ShopScene";
        public const string PROFILE_SCENE = "ProfileScene";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadTimer()
        {
            LoadScene(TIMER_SCENE);
        }

        public void LoadHome()
        {
            LoadScene(HOME_SCENE);
        }

        public void LoadShop()
        {
            LoadScene(SHOP_SCENE);
        }

        public void LoadProfile()
        {
            LoadScene(PROFILE_SCENE);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneAsync(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }

        public string GetCurrentScene()
        {
            return SceneManager.GetActiveScene().name;
        }
    }
}
