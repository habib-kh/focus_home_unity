using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FocusHome
{
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { get; private set; }

        [Header("Shop Data")]
        [SerializeField] private List<FurnitureData> allFurniture = new List<FurnitureData>();

        [Header("Events")]
        public UnityEvent<FurnitureData> OnItemPurchased;
        public UnityEvent OnShopRefreshed;

        private HashSet<string> ownedFurnitureIds = new HashSet<string>();

        private const string OWNED_FURNITURE_KEY = "OwnedFurniture";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            LoadOwnedFurniture();
        }

        public List<FurnitureData> GetAllFurniture()
        {
            return new List<FurnitureData>(allFurniture);
        }

        public List<FurnitureData> GetFurnitureByCategory(FurnitureCategory category)
        {
            return allFurniture.FindAll(f => f.Category == category);
        }

        public List<FurnitureData> GetOwnedFurniture()
        {
            return allFurniture.FindAll(f => ownedFurnitureIds.Contains(f.Id));
        }

        public List<FurnitureData> GetAvailableForPurchase()
        {
            return allFurniture.FindAll(f => !ownedFurnitureIds.Contains(f.Id));
        }

        public bool IsOwned(string furnitureId)
        {
            return ownedFurnitureIds.Contains(furnitureId);
        }

        public bool TryPurchase(FurnitureData furniture)
        {
            if (furniture == null) return false;

            // Already owned
            if (ownedFurnitureIds.Contains(furniture.Id))
            {
                Debug.Log($"Already own {furniture.FurnitureName}");
                return false;
            }

            // Check if can afford
            if (CoinManager.Instance == null || !CoinManager.Instance.CanAfford(furniture.Price))
            {
                Debug.Log($"Cannot afford {furniture.FurnitureName} (costs {furniture.Price})");
                return false;
            }

            // Make purchase
            CoinManager.Instance.SpendCoins(furniture.Price);
            ownedFurnitureIds.Add(furniture.Id);
            SaveOwnedFurniture();

            OnItemPurchased?.Invoke(furniture);
            Debug.Log($"Purchased {furniture.FurnitureName} for {furniture.Price} coins");

            return true;
        }

        public void UnlockFurnitureFree(string furnitureId)
        {
            if (!ownedFurnitureIds.Contains(furnitureId))
            {
                ownedFurnitureIds.Add(furnitureId);
                SaveOwnedFurniture();

                var furniture = allFurniture.Find(f => f.Id == furnitureId);
                if (furniture != null)
                {
                    OnItemPurchased?.Invoke(furniture);
                }
            }
        }

        public void UnlockStarterFurniture()
        {
            // Give player some basic furniture to start
            var starterItems = new[] { "rug_basic", "lamp_floor", "plant_small" };
            foreach (var itemId in starterItems)
            {
                UnlockFurnitureFree(itemId);
            }
        }

        private void SaveOwnedFurniture()
        {
            string[] ids = new string[ownedFurnitureIds.Count];
            ownedFurnitureIds.CopyTo(ids);
            string json = JsonUtility.ToJson(new OwnedFurnitureData { ids = ids });
            PlayerPrefs.SetString(OWNED_FURNITURE_KEY, json);
            PlayerPrefs.Save();
        }

        private void LoadOwnedFurniture()
        {
            ownedFurnitureIds.Clear();

            if (PlayerPrefs.HasKey(OWNED_FURNITURE_KEY))
            {
                string json = PlayerPrefs.GetString(OWNED_FURNITURE_KEY);
                var data = JsonUtility.FromJson<OwnedFurnitureData>(json);
                if (data?.ids != null)
                {
                    foreach (var id in data.ids)
                    {
                        ownedFurnitureIds.Add(id);
                    }
                }
            }
        }

        [System.Serializable]
        private class OwnedFurnitureData
        {
            public string[] ids;
        }

        #if UNITY_EDITOR
        [ContextMenu("Unlock All Furniture (Debug)")]
        private void DebugUnlockAll()
        {
            foreach (var furniture in allFurniture)
            {
                ownedFurnitureIds.Add(furniture.Id);
            }
            SaveOwnedFurniture();
            Debug.Log("Unlocked all furniture!");
        }

        [ContextMenu("Reset Owned Furniture (Debug)")]
        private void DebugResetOwned()
        {
            ownedFurnitureIds.Clear();
            PlayerPrefs.DeleteKey(OWNED_FURNITURE_KEY);
            Debug.Log("Reset owned furniture!");
        }
        #endif
    }
}
