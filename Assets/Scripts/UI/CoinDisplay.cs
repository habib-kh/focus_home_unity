using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FocusHome
{
    public class CoinDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private Image coinIcon;
        [SerializeField] private Animator animator;

        private CoinManager coinManager;
        private int lastDisplayedCoins;

        private void Start()
        {
            coinManager = CoinManager.Instance;

            if (coinManager != null)
            {
                coinManager.OnCoinsChanged.AddListener(UpdateDisplay);
                coinManager.OnCoinsAdded.AddListener(OnCoinsAdded);
                UpdateDisplay(coinManager.CurrentCoins);
            }
        }

        private void UpdateDisplay(int coins)
        {
            if (coinsText != null)
            {
                coinsText.text = coins.ToString("N0");
            }
            lastDisplayedCoins = coins;
        }

        private void OnCoinsAdded(int amount)
        {
            // Play animation or effect when coins are added
            if (animator != null)
            {
                animator.SetTrigger("CoinAdded");
            }

            // Could also spawn floating text showing +amount
        }

        private void OnDestroy()
        {
            if (coinManager != null)
            {
                coinManager.OnCoinsChanged.RemoveListener(UpdateDisplay);
                coinManager.OnCoinsAdded.RemoveListener(OnCoinsAdded);
            }
        }
    }
}
