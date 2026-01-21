using UnityEngine;
using UnityEngine.SceneManagement;

namespace FocusHome
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        public CoinManager coinManager;
        public FocusTimer focusTimer;
        public HomeManager homeManager;

        [Header("Settings")]
        public int coinsPerMinute = 1;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeGame()
        {
            // Load saved data
            LoadGameData();
        }

        public void LoadGameData()
        {
            // Load coins
            int savedCoins = PlayerPrefs.GetInt("Coins", 0);
            if (coinManager != null)
            {
                coinManager.SetCoins(savedCoins);
            }

            // Load total focus minutes
            int totalMinutes = PlayerPrefs.GetInt("TotalFocusMinutes", 0);

            // Load streak
            int currentStreak = PlayerPrefs.GetInt("CurrentStreak", 0);

            Debug.Log($"Game loaded - Coins: {savedCoins}, Total Minutes: {totalMinutes}, Streak: {currentStreak}");
        }

        public void SaveGameData()
        {
            if (coinManager != null)
            {
                PlayerPrefs.SetInt("Coins", coinManager.CurrentCoins);
            }
            PlayerPrefs.Save();
            Debug.Log("Game data saved");
        }

        public void OnFocusSessionComplete(int minutes)
        {
            int earnedCoins = minutes * coinsPerMinute;
            coinManager?.AddCoins(earnedCoins);

            // Update total focus minutes
            int totalMinutes = PlayerPrefs.GetInt("TotalFocusMinutes", 0) + minutes;
            PlayerPrefs.SetInt("TotalFocusMinutes", totalMinutes);

            // Update streak
            UpdateStreak();

            SaveGameData();

            Debug.Log($"Session complete! Earned {earnedCoins} coins for {minutes} minutes");
        }

        public void OnFocusSessionFailed()
        {
            Debug.Log("Focus session failed - no coins earned");
        }

        private void UpdateStreak()
        {
            string lastFocusDate = PlayerPrefs.GetString("LastFocusDate", "");
            string today = System.DateTime.Now.ToString("yyyy-MM-dd");
            string yesterday = System.DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            int currentStreak = PlayerPrefs.GetInt("CurrentStreak", 0);

            if (lastFocusDate == today)
            {
                // Already focused today, no change
            }
            else if (lastFocusDate == yesterday)
            {
                // Consecutive day
                currentStreak++;
                PlayerPrefs.SetInt("CurrentStreak", currentStreak);
            }
            else
            {
                // Streak broken or first time
                currentStreak = 1;
                PlayerPrefs.SetInt("CurrentStreak", currentStreak);
            }

            PlayerPrefs.SetString("LastFocusDate", today);

            // Update longest streak
            int longestStreak = PlayerPrefs.GetInt("LongestStreak", 0);
            if (currentStreak > longestStreak)
            {
                PlayerPrefs.SetInt("LongestStreak", currentStreak);
            }
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveGameData();
            }
        }

        private void OnApplicationQuit()
        {
            SaveGameData();
        }
    }
}
