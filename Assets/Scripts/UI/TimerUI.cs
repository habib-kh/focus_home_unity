using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FocusHome
{
    public class TimerUI : MonoBehaviour
    {
        [Header("Timer Display")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Image progressCircle;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Duration Selection")]
        [SerializeField] private GameObject durationPanel;
        [SerializeField] private Button[] durationButtons;
        [SerializeField] private TMP_InputField customDurationInput;

        [Header("Control Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button cancelButton;

        [Header("Completion Panel")]
        [SerializeField] private GameObject completionPanel;
        [SerializeField] private TextMeshProUGUI earnedCoinsText;
        [SerializeField] private Button continueButton;

        [Header("Failed Panel")]
        [SerializeField] private GameObject failedPanel;
        [SerializeField] private Button retryButton;

        private FocusTimer focusTimer;

        private void Start()
        {
            focusTimer = FocusTimer.Instance;

            if (focusTimer != null)
            {
                // Subscribe to events
                focusTimer.OnTimerTick.AddListener(UpdateTimerDisplay);
                focusTimer.OnProgressChanged.AddListener(UpdateProgressCircle);
                focusTimer.OnTimerStarted.AddListener(OnTimerStarted);
                focusTimer.OnTimerPaused.AddListener(OnTimerPaused);
                focusTimer.OnTimerResumed.AddListener(OnTimerResumed);
                focusTimer.OnTimerCompleted.AddListener(OnTimerCompleted);
                focusTimer.OnTimerFailed.AddListener(OnTimerFailed);
            }

            SetupButtons();
            ShowDurationSelection();
        }

        private void SetupButtons()
        {
            // Duration buttons
            if (focusTimer != null && durationButtons != null)
            {
                int[] presets = focusTimer.PresetDurations;
                for (int i = 0; i < durationButtons.Length && i < presets.Length; i++)
                {
                    int duration = presets[i];
                    int index = i;
                    durationButtons[i].onClick.AddListener(() => SelectDuration(duration));

                    // Update button text
                    var buttonText = durationButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.text = $"{duration} min";
                    }
                }
            }

            // Control buttons
            startButton?.onClick.AddListener(StartTimer);
            pauseButton?.onClick.AddListener(PauseTimer);
            resumeButton?.onClick.AddListener(ResumeTimer);
            cancelButton?.onClick.AddListener(CancelTimer);
            continueButton?.onClick.AddListener(OnContinueAfterCompletion);
            retryButton?.onClick.AddListener(OnRetryAfterFail);

            // Custom duration
            customDurationInput?.onEndEdit.AddListener(OnCustomDurationEntered);
        }

        private void SelectDuration(int minutes)
        {
            focusTimer?.SetDuration(minutes);
            UpdateTimerDisplay(minutes * 60);

            if (statusText != null)
            {
                statusText.text = $"Ready for {minutes} minutes";
            }
        }

        private void OnCustomDurationEntered(string value)
        {
            if (int.TryParse(value, out int minutes) && minutes > 0)
            {
                SelectDuration(Mathf.Min(minutes, 180)); // Max 3 hours
            }
        }

        private void StartTimer()
        {
            focusTimer?.StartTimer();
        }

        private void PauseTimer()
        {
            focusTimer?.PauseTimer();
        }

        private void ResumeTimer()
        {
            focusTimer?.ResumeTimer();
        }

        private void CancelTimer()
        {
            focusTimer?.StopTimer();
            ShowDurationSelection();
        }

        private void UpdateTimerDisplay(float remainingSeconds)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(remainingSeconds / 60);
                int seconds = Mathf.FloorToInt(remainingSeconds % 60);
                timerText.text = $"{minutes:00}:{seconds:00}";
            }
        }

        private void UpdateProgressCircle(float progress)
        {
            if (progressCircle != null)
            {
                progressCircle.fillAmount = progress;
            }
        }

        private void OnTimerStarted()
        {
            ShowTimerRunning();

            if (statusText != null)
            {
                statusText.text = "Stay focused!";
            }
        }

        private void OnTimerPaused()
        {
            ShowTimerPaused();

            if (statusText != null)
            {
                statusText.text = "Paused";
            }
        }

        private void OnTimerResumed()
        {
            ShowTimerRunning();

            if (statusText != null)
            {
                statusText.text = "Stay focused!";
            }
        }

        private void OnTimerCompleted()
        {
            ShowCompletionPanel();

            if (earnedCoinsText != null && focusTimer != null)
            {
                int earnedCoins = focusTimer.SelectedDurationMinutes * GameManager.Instance.coinsPerMinute;
                earnedCoinsText.text = $"+{earnedCoins} coins!";
            }
        }

        private void OnTimerFailed()
        {
            ShowFailedPanel();
        }

        private void OnContinueAfterCompletion()
        {
            completionPanel?.SetActive(false);
            ShowDurationSelection();
        }

        private void OnRetryAfterFail()
        {
            failedPanel?.SetActive(false);
            ShowDurationSelection();
        }

        // UI State Management
        private void ShowDurationSelection()
        {
            durationPanel?.SetActive(true);
            startButton?.gameObject.SetActive(true);
            pauseButton?.gameObject.SetActive(false);
            resumeButton?.gameObject.SetActive(false);
            cancelButton?.gameObject.SetActive(false);
            completionPanel?.SetActive(false);
            failedPanel?.SetActive(false);

            if (statusText != null)
            {
                statusText.text = "Select duration";
            }

            if (timerText != null)
            {
                timerText.text = "00:00";
            }

            if (progressCircle != null)
            {
                progressCircle.fillAmount = 0;
            }
        }

        private void ShowTimerRunning()
        {
            durationPanel?.SetActive(false);
            startButton?.gameObject.SetActive(false);
            pauseButton?.gameObject.SetActive(true);
            resumeButton?.gameObject.SetActive(false);
            cancelButton?.gameObject.SetActive(true);
        }

        private void ShowTimerPaused()
        {
            pauseButton?.gameObject.SetActive(false);
            resumeButton?.gameObject.SetActive(true);
        }

        private void ShowCompletionPanel()
        {
            durationPanel?.SetActive(false);
            startButton?.gameObject.SetActive(false);
            pauseButton?.gameObject.SetActive(false);
            resumeButton?.gameObject.SetActive(false);
            cancelButton?.gameObject.SetActive(false);
            completionPanel?.SetActive(true);
        }

        private void ShowFailedPanel()
        {
            durationPanel?.SetActive(false);
            startButton?.gameObject.SetActive(false);
            pauseButton?.gameObject.SetActive(false);
            resumeButton?.gameObject.SetActive(false);
            cancelButton?.gameObject.SetActive(false);
            failedPanel?.SetActive(true);
        }

        private void OnDestroy()
        {
            if (focusTimer != null)
            {
                focusTimer.OnTimerTick.RemoveListener(UpdateTimerDisplay);
                focusTimer.OnProgressChanged.RemoveListener(UpdateProgressCircle);
                focusTimer.OnTimerStarted.RemoveListener(OnTimerStarted);
                focusTimer.OnTimerPaused.RemoveListener(OnTimerPaused);
                focusTimer.OnTimerResumed.RemoveListener(OnTimerResumed);
                focusTimer.OnTimerCompleted.RemoveListener(OnTimerCompleted);
                focusTimer.OnTimerFailed.RemoveListener(OnTimerFailed);
            }
        }
    }
}
