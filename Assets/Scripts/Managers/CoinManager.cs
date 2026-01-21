using UnityEngine;
using UnityEngine.Events;

namespace FocusHome
{
    public class CoinManager : MonoBehaviour
    {
        public static CoinManager Instance { get; private set; }

        [SerializeField] private int currentCoins = 0;

        public int CurrentCoins => currentCoins;

        // Events
        public UnityEvent<int> OnCoinsChanged;
        public UnityEvent<int> OnCoinsAdded;
        public UnityEvent<int> OnCoinsSpent;

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

        public void SetCoins(int amount)
        {
            currentCoins = amount;
            OnCoinsChanged?.Invoke(currentCoins);
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0) return;

            currentCoins += amount;
            OnCoinsAdded?.Invoke(amount);
            OnCoinsChanged?.Invoke(currentCoins);

            Debug.Log($"Added {amount} coins. Total: {currentCoins}");
        }

        public bool SpendCoins(int amount)
        {
            if (amount <= 0 || currentCoins < amount)
            {
                Debug.Log($"Cannot spend {amount} coins. Current: {currentCoins}");
                return false;
            }

            currentCoins -= amount;
            OnCoinsSpent?.Invoke(amount);
            OnCoinsChanged?.Invoke(currentCoins);

            Debug.Log($"Spent {amount} coins. Remaining: {currentCoins}");
            return true;
        }

        public bool CanAfford(int amount)
        {
            return currentCoins >= amount;
        }

        // For testing
        [ContextMenu("Add 100 Coins")]
        public void AddTestCoins()
        {
            AddCoins(100);
        }

        [ContextMenu("Add 10000 Coins")]
        public void AddLotsOfCoins()
        {
            AddCoins(10000);
        }
    }
}
