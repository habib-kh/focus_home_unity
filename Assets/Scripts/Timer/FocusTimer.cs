using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace FocusHome
{
    public enum TimerState
    {
        Idle,
        Running,
        Paused,
        Completed,
        Failed
    }

    public class FocusTimer : MonoBehaviour
    {
        public static FocusTimer Instance { get; private set; }

        [Header("Timer Settings")]
        [SerializeField] private int[] presetDurations = { 15, 25, 45, 60 }; // minutes
        [SerializeField] private int selectedDurationMinutes = 25;

        [Header("State")]
        [SerializeField] private TimerState currentState = TimerState.Idle;
        [SerializeField] private float remainingSeconds;
        [SerializeField] private float totalSeconds;

        // Events
        public UnityEvent OnTimerStarted;
        public UnityEvent OnTimerPaused;
        public UnityEvent OnTimerResumed;
        public UnityEvent OnTimerCompleted;
        public UnityEvent OnTimerFailed;
        public UnityEvent<float> OnTimerTick; // Passes remaining seconds
        public UnityEvent<float> OnProgressChanged; // Passes progress 0-1

        // Properties
        public TimerState CurrentState => currentState;
        public float RemainingSeconds => remainingSeconds;
        public float TotalSeconds => totalSeconds;
        public int SelectedDurationMinutes => selectedDurationMinutes;
        public int[] PresetDurations => presetDurations;
        public float Progress => totalSeconds > 0 ? 1f - (remainingSeconds / totalSeconds) : 0f;

        public string RemainingTimeFormatted
        {
            get
            {
                int minutes = Mathf.FloorToInt(remainingSeconds / 60);
                int seconds = Mathf.FloorToInt(remainingSeconds % 60);
                return $"{minutes:00}:{seconds:00}";
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Keep screen on during timer (Unity equivalent of wakelock)
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public void SetDuration(int minutes)
        {
            if (currentState == TimerState.Running) return;

            selectedDurationMinutes = Mathf.Max(1, minutes);
            remainingSeconds = selectedDurationMinutes * 60f;
            totalSeconds = remainingSeconds;
        }

        public void StartTimer()
        {
            if (currentState == TimerState.Running) return;

            totalSeconds = selectedDurationMinutes * 60f;
            remainingSeconds = totalSeconds;
            currentState = TimerState.Running;

            OnTimerStarted?.Invoke();
            StartCoroutine(TimerCoroutine());

            Debug.Log($"Timer started: {selectedDurationMinutes} minutes");
        }

        public void PauseTimer()
        {
            if (currentState != TimerState.Running) return;

            currentState = TimerState.Paused;
            OnTimerPaused?.Invoke();

            Debug.Log("Timer paused");
        }

        public void ResumeTimer()
        {
            if (currentState != TimerState.Paused) return;

            currentState = TimerState.Running;
            OnTimerResumed?.Invoke();
            StartCoroutine(TimerCoroutine());

            Debug.Log("Timer resumed");
        }

        public void StopTimer()
        {
            StopAllCoroutines();
            currentState = TimerState.Idle;
            remainingSeconds = 0;

            Debug.Log("Timer stopped");
        }

        public void FailTimer()
        {
            if (currentState != TimerState.Running && currentState != TimerState.Paused) return;

            StopAllCoroutines();
            currentState = TimerState.Failed;
            OnTimerFailed?.Invoke();

            // Notify game manager
            GameManager.Instance?.OnFocusSessionFailed();

            Debug.Log("Timer failed - user left app");
        }

        private IEnumerator TimerCoroutine()
        {
            while (currentState == TimerState.Running && remainingSeconds > 0)
            {
                yield return new WaitForSeconds(1f);

                if (currentState == TimerState.Running)
                {
                    remainingSeconds -= 1f;
                    remainingSeconds = Mathf.Max(0, remainingSeconds);

                    OnTimerTick?.Invoke(remainingSeconds);
                    OnProgressChanged?.Invoke(Progress);
                }
            }

            if (remainingSeconds <= 0 && currentState == TimerState.Running)
            {
                CompleteTimer();
            }
        }

        private void CompleteTimer()
        {
            currentState = TimerState.Completed;
            OnTimerCompleted?.Invoke();

            // Notify game manager
            GameManager.Instance?.OnFocusSessionComplete(selectedDurationMinutes);

            Debug.Log("Timer completed!");
        }

        // Called when app loses focus (for penalty system)
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && currentState == TimerState.Running)
            {
                // App lost focus during timer - FAIL the session
                FailTimer();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && currentState == TimerState.Running)
            {
                // App was paused during timer - FAIL the session
                FailTimer();
            }
        }
    }
}
