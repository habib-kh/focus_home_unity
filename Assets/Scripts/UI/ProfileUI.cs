using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FocusHome
{
    public class ProfileUI : MonoBehaviour
    {
        [Header("Stats Display")]
        [SerializeField] private TextMeshProUGUI totalFocusTimeText;
        [SerializeField] private TextMeshProUGUI currentStreakText;
        [SerializeField] private TextMeshProUGUI sessionsCompletedText;
        [SerializeField] private TextMeshProUGUI totalCoinsEarnedText;
        [SerializeField] private TextMeshProUGUI furnitureOwnedText;

        [Header("Achievements")]
        [SerializeField] private Transform achievementContainer;
        [SerializeField] private GameObject achievementItemPrefab;

        [Header("Settings")]
        [SerializeField] private Toggle soundToggle;
        [SerializeField] private Toggle notificationToggle;
        [SerializeField] private Slider volumeSlider;

        [Header("Buttons")]
        [SerializeField] private Button resetDataButton;

        private const string SOUND_ENABLED_KEY = "SoundEnabled";
        private const string NOTIFICATIONS_KEY = "NotificationsEnabled";
        private const string VOLUME_KEY = "Volume";

        private void Start()
        {
            LoadSettings();
            RefreshStats();
            SetupListeners();
        }

        private void SetupListeners()
        {
            if (soundToggle != null)
                soundToggle.onValueChanged.AddListener(OnSoundToggled);

            if (notificationToggle != null)
                notificationToggle.onValueChanged.AddListener(OnNotificationsToggled);

            if (volumeSlider != null)
                volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

            if (resetDataButton != null)
                resetDataButton.onClick.AddListener(OnResetDataClicked);
        }

        public void RefreshStats()
        {
            if (GameManager.Instance == null) return;

            var data = GameManager.Instance.GetPlayerData();

            if (totalFocusTimeText != null)
            {
                int hours = data.totalFocusMinutes / 60;
                int minutes = data.totalFocusMinutes % 60;
                totalFocusTimeText.text = $"Total Focus Time: {hours}h {minutes}m";
            }

            if (currentStreakText != null)
            {
                currentStreakText.text = $"Current Streak: {data.currentStreak} days";
            }

            if (sessionsCompletedText != null)
            {
                sessionsCompletedText.text = $"Sessions Completed: {data.sessionsCompleted}";
            }

            if (totalCoinsEarnedText != null)
            {
                totalCoinsEarnedText.text = $"Total Coins Earned: {data.totalCoinsEarned}";
            }

            if (furnitureOwnedText != null && ShopManager.Instance != null)
            {
                int ownedCount = ShopManager.Instance.GetOwnedFurniture().Count;
                int totalCount = ShopManager.Instance.GetAllFurniture().Count;
                furnitureOwnedText.text = $"Furniture Owned: {ownedCount}/{totalCount}";
            }
        }

        private void LoadSettings()
        {
            if (soundToggle != null)
            {
                soundToggle.isOn = PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1;
            }

            if (notificationToggle != null)
            {
                notificationToggle.isOn = PlayerPrefs.GetInt(NOTIFICATIONS_KEY, 1) == 1;
            }

            if (volumeSlider != null)
            {
                volumeSlider.value = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
                AudioListener.volume = volumeSlider.value;
            }
        }

        private void OnSoundToggled(bool enabled)
        {
            PlayerPrefs.SetInt(SOUND_ENABLED_KEY, enabled ? 1 : 0);
            AudioListener.volume = enabled ? PlayerPrefs.GetFloat(VOLUME_KEY, 1f) : 0f;
            PlayerPrefs.Save();
        }

        private void OnNotificationsToggled(bool enabled)
        {
            PlayerPrefs.SetInt(NOTIFICATIONS_KEY, enabled ? 1 : 0);
            PlayerPrefs.Save();
            // TODO: Implement actual notification scheduling
        }

        private void OnVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat(VOLUME_KEY, value);
            if (soundToggle == null || soundToggle.isOn)
            {
                AudioListener.volume = value;
            }
            PlayerPrefs.Save();
        }

        private void OnResetDataClicked()
        {
            // Show confirmation dialog in real app
            Debug.Log("Reset data requested - implement confirmation dialog");

            // For now, just reset
            // PlayerPrefs.DeleteAll();
            // GameManager.Instance?.ResetAllData();
            // RefreshStats();
        }

        // Achievement display helpers
        public void DisplayAchievement(string name, string description, bool unlocked)
        {
            if (achievementContainer == null || achievementItemPrefab == null) return;

            var item = Instantiate(achievementItemPrefab, achievementContainer);

            var nameText = item.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
            var descText = item.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();
            var icon = item.transform.Find("Icon")?.GetComponent<Image>();

            if (nameText != null) nameText.text = name;
            if (descText != null) descText.text = description;
            if (icon != null) icon.color = unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }
}
