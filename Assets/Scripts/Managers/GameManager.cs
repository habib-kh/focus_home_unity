using UnityEngine;
using UnityEngine.SceneManagement;

namespace FocusHome
{
    [System.Serializable]
    public struct PlayerData
    {
        public int totalFocusMinutes;
        public int currentStreak;
        public int longestStreak;
        public int sessionsCompleted;
        public int totalCoinsEarned;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        public CoinManager coinManager;
        public FocusTimer focusTimer;
        public HomeManager homeManager;

        [Header("Settings")]
        public int coinsPerMinute = 1;

        private PlayerData playerData;

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
            LoadGameData();
        }

        public PlayerData GetPlayerData()
        {
            return playerData;
        }

        public void LoadGameData()
        {
            // Load coins
            int savedCoins = PlayerPrefs.GetInt("Coins", 0);
            if (coinManager != null)
            {
                coinManager.SetCoins(savedCoins);
            }

            // Load player data
            playerData.totalFocusMinutes = PlayerPrefs.GetInt("TotalFocusMinutes", 0);
            playerData.currentStreak = PlayerPrefs.GetInt("CurrentStreak", 0);
            playerData.longestStreak = PlayerPrefs.GetInt("LongestStreak", 0);
            playerData.sessionsCompleted = PlayerPrefs.GetInt("SessionsCompleted", 0);
            playerData.totalCoinsEarned = PlayerPrefs.GetInt("TotalCoinsEarned", 0);

            Debug.Log($"Game loaded - Coins: {savedCoins}, Total Minutes: {playerData.totalFocusMinutes}, Streak: {playerData.currentStreak}");
        }

        public void SaveGameData()
        {
            if (coinManager != null)
            {
                PlayerPrefs.SetInt("Coins", coinManager.CurrentCoins);
            }

            PlayerPrefs.SetInt("TotalFocusMinutes", playerData.totalFocusMinutes);
            PlayerPrefs.SetInt("CurrentStreak", playerData.currentStreak);
            PlayerPrefs.SetInt("LongestStreak", playerData.longestStreak);
            PlayerPrefs.SetInt("SessionsCompleted", playerData.sessionsCompleted);
            PlayerPrefs.SetInt("TotalCoinsEarned", playerData.totalCoinsEarned);

            PlayerPrefs.Save();
            Debug.Log("Game data saved");
        }

        public void OnFocusSessionComplete(int minutes)
        {
            int earnedCoins = minutes * coinsPerMinute;
            coinManager?.AddCoins(earnedCoins);

            // Update player data
            playerData.totalFocusMinutes += minutes;
            playerData.sessionsCompleted++;
            playerData.totalCoinsEarned += earnedCoins;

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

            if (lastFocusDate == today)
            {
                // Already focused today, no change
            }
            else if (lastFocusDate == yesterday)
            {
                // Consecutive day
                playerData.currentStreak++;
            }
            else
            {
                // Streak broken or first time
                playerData.currentStreak = 1;
            }

            PlayerPrefs.SetString("LastFocusDate", today);

            // Update longest streak
            if (playerData.currentStreak > playerData.longestStreak)
            {
                playerData.longestStreak = playerData.currentStreak;
            }
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void ResetAllData()
        {
            playerData = new PlayerData();
            if (coinManager != null)
            {
                coinManager.SetCoins(0);
            }
            PlayerPrefs.DeleteAll();
            SaveGameData();
            Debug.Log("All data reset");
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
