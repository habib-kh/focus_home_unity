using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace FocusHome
{
    public class ShopUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform itemGrid;
        [SerializeField] private GameObject shopItemPrefab;
        [SerializeField] private TMP_Dropdown categoryFilter;
        [SerializeField] private Toggle showOwnedToggle;

        [Header("Item Detail Panel")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private Image detailImage;
        [SerializeField] private TextMeshProUGUI detailName;
        [SerializeField] private TextMeshProUGUI detailPrice;
        [SerializeField] private TextMeshProUGUI detailDescription;
        [SerializeField] private Button buyButton;
        [SerializeField] private TextMeshProUGUI buyButtonText;

        private FurnitureData selectedFurniture;
        private List<GameObject> spawnedItems = new List<GameObject>();

        private void Start()
        {
            SetupCategoryFilter();
            RefreshShop();

            if (categoryFilter != null)
                categoryFilter.onValueChanged.AddListener(_ => RefreshShop());

            if (showOwnedToggle != null)
                showOwnedToggle.onValueChanged.AddListener(_ => RefreshShop());

            if (buyButton != null)
                buyButton.onClick.AddListener(OnBuyClicked);

            HideDetailPanel();
        }

        private void SetupCategoryFilter()
        {
            if (categoryFilter == null) return;

            categoryFilter.ClearOptions();

            var options = new List<string> { "All" };
            foreach (FurnitureCategory cat in System.Enum.GetValues(typeof(FurnitureCategory)))
            {
                options.Add(cat.ToString());
            }

            categoryFilter.AddOptions(options);
        }

        public void RefreshShop()
        {
            // Clear existing items
            foreach (var item in spawnedItems)
            {
                Destroy(item);
            }
            spawnedItems.Clear();

            if (ShopManager.Instance == null || shopItemPrefab == null || itemGrid == null)
                return;

            // Get filtered furniture list
            List<FurnitureData> furniture;

            int categoryIndex = categoryFilter != null ? categoryFilter.value : 0;
            if (categoryIndex == 0) // All
            {
                furniture = ShopManager.Instance.GetAllFurniture();
            }
            else
            {
                var category = (FurnitureCategory)(categoryIndex - 1);
                furniture = ShopManager.Instance.GetFurnitureByCategory(category);
            }

            // Filter by ownership
            bool showOwned = showOwnedToggle == null || showOwnedToggle.isOn;
            if (!showOwned)
            {
                furniture = furniture.FindAll(f => !ShopManager.Instance.IsOwned(f.Id));
            }

            // Create shop item UI for each furniture
            foreach (var item in furniture)
            {
                var shopItem = Instantiate(shopItemPrefab, itemGrid);
                spawnedItems.Add(shopItem);

                SetupShopItem(shopItem, item);
            }
        }

        private void SetupShopItem(GameObject shopItem, FurnitureData data)
        {
            // Find UI components
            var image = shopItem.GetComponentInChildren<Image>();
            var nameText = shopItem.GetComponentInChildren<TextMeshProUGUI>();
            var button = shopItem.GetComponent<Button>();

            if (image != null && data.Sprite != null)
            {
                image.sprite = data.Sprite;
            }

            if (nameText != null)
            {
                bool owned = ShopManager.Instance.IsOwned(data.Id);
                nameText.text = owned ? $"{data.FurnitureName}\n(Owned)" : $"{data.FurnitureName}\n{data.Price} coins";
            }

            if (button != null)
            {
                button.onClick.AddListener(() => ShowItemDetail(data));
            }
        }

        public void ShowItemDetail(FurnitureData furniture)
        {
            selectedFurniture = furniture;

            if (detailPanel != null)
                detailPanel.SetActive(true);

            if (detailImage != null && furniture.Sprite != null)
                detailImage.sprite = furniture.Sprite;

            if (detailName != null)
                detailName.text = furniture.FurnitureName;

            if (detailPrice != null)
                detailPrice.text = $"{furniture.Price} coins";

            if (detailDescription != null)
                detailDescription.text = $"Category: {furniture.Category}\nSize: {furniture.GridSize.x}x{furniture.GridSize.y}";

            UpdateBuyButton();
        }

        private void UpdateBuyButton()
        {
            if (buyButton == null || selectedFurniture == null) return;

            bool owned = ShopManager.Instance != null && ShopManager.Instance.IsOwned(selectedFurniture.Id);
            bool canAfford = CoinManager.Instance != null && CoinManager.Instance.CanAfford(selectedFurniture.Price);

            buyButton.interactable = !owned && canAfford;

            if (buyButtonText != null)
            {
                if (owned)
                    buyButtonText.text = "Owned";
                else if (!canAfford)
                    buyButtonText.text = "Can't Afford";
                else
                    buyButtonText.text = $"Buy ({selectedFurniture.Price})";
            }
        }

        public void HideDetailPanel()
        {
            if (detailPanel != null)
                detailPanel.SetActive(false);

            selectedFurniture = null;
        }

        private void OnBuyClicked()
        {
            if (selectedFurniture == null || ShopManager.Instance == null)
                return;

            if (ShopManager.Instance.TryPurchase(selectedFurniture))
            {
                UpdateBuyButton();
                RefreshShop();
            }
        }
    }
}
