using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace FocusHome
{
    public class HomeUI : MonoBehaviour
    {
        [Header("Top Bar")]
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private Button editButton;
        [SerializeField] private Button doneButton;

        [Header("Edit Mode Panel")]
        [SerializeField] private GameObject editModeIndicator;
        [SerializeField] private GameObject furnitureActionPanel;
        [SerializeField] private Button rotateButton;
        [SerializeField] private Button removeButton;

        [Header("Furniture Palette")]
        [SerializeField] private GameObject furniturePalette;
        [SerializeField] private Transform furnitureListContent;
        [SerializeField] private GameObject furnitureItemPrefab;

        [Header("Shop")]
        [SerializeField] private Button shopButton;
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private Transform shopListContent;
        [SerializeField] private GameObject shopItemPrefab;
        [SerializeField] private Button closeShopButton;

        [Header("Navigation")]
        [SerializeField] private Button timerButton;
        [SerializeField] private Button profileButton;

        private HomeManager homeManager;
        private CoinManager coinManager;

        private void Start()
        {
            homeManager = HomeManager.Instance;
            coinManager = CoinManager.Instance;

            SetupEventListeners();
            SetupUI();
            UpdateCoinsDisplay();
            PopulateFurniturePalette();
        }

        private void SetupEventListeners()
        {
            // Home Manager events
            if (homeManager != null)
            {
                homeManager.OnEditModeChanged.AddListener(OnEditModeChanged);
                homeManager.OnFurnitureSelected.AddListener(OnFurnitureSelected);
            }

            // Coin Manager events
            if (coinManager != null)
            {
                coinManager.OnCoinsChanged.AddListener(OnCoinsChanged);
            }

            // Button listeners
            editButton?.onClick.AddListener(EnterEditMode);
            doneButton?.onClick.AddListener(ExitEditMode);
            rotateButton?.onClick.AddListener(RotateSelected);
            removeButton?.onClick.AddListener(RemoveSelected);
            shopButton?.onClick.AddListener(OpenShop);
            closeShopButton?.onClick.AddListener(CloseShop);

            // Navigation
            timerButton?.onClick.AddListener(() => GameManager.Instance?.LoadScene("TimerScene"));
            profileButton?.onClick.AddListener(() => GameManager.Instance?.LoadScene("ProfileScene"));
        }

        private void SetupUI()
        {
            // Initial state - not in edit mode
            editButton?.gameObject.SetActive(true);
            doneButton?.gameObject.SetActive(false);
            editModeIndicator?.SetActive(false);
            furniturePalette?.SetActive(false);
            furnitureActionPanel?.SetActive(false);
            shopPanel?.SetActive(false);
        }

        private void UpdateCoinsDisplay()
        {
            if (coinsText != null && coinManager != null)
            {
                coinsText.text = coinManager.CurrentCoins.ToString("N0");
            }
        }

        private void OnCoinsChanged(int newAmount)
        {
            if (coinsText != null)
            {
                coinsText.text = newAmount.ToString("N0");
            }
        }

        private void EnterEditMode()
        {
            homeManager?.SetEditMode(true);
        }

        private void ExitEditMode()
        {
            homeManager?.SetEditMode(false);
        }

        private void OnEditModeChanged(bool isEditMode)
        {
            editButton?.gameObject.SetActive(!isEditMode);
            doneButton?.gameObject.SetActive(isEditMode);
            editModeIndicator?.SetActive(isEditMode);
            furniturePalette?.SetActive(isEditMode);

            if (!isEditMode)
            {
                furnitureActionPanel?.SetActive(false);
            }
        }

        private void OnFurnitureSelected(FurnitureItem furniture)
        {
            furnitureActionPanel?.SetActive(furniture != null);
        }

        private void RotateSelected()
        {
            homeManager?.RotateSelectedFurniture();
        }

        private void RemoveSelected()
        {
            homeManager?.RemoveSelectedFurniture();
            furnitureActionPanel?.SetActive(false);
        }

        private void PopulateFurniturePalette()
        {
            if (homeManager == null || furnitureListContent == null || furnitureItemPrefab == null)
                return;

            // Clear existing items
            foreach (Transform child in furnitureListContent)
            {
                Destroy(child.gameObject);
            }

            // Get owned furniture (for now, show all)
            foreach (var furnitureData in homeManager.FurnitureCatalog)
            {
                CreatePaletteItem(furnitureData);
            }
        }

        private void CreatePaletteItem(FurnitureData data)
        {
            GameObject item = Instantiate(furnitureItemPrefab, furnitureListContent);

            // Set up the item
            var image = item.GetComponentInChildren<Image>();
            if (image != null && data.previewSprite != null)
            {
                image.sprite = data.previewSprite;
            }

            var nameText = item.GetComponentInChildren<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = data.furnitureName;
            }

            var button = item.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => PlaceFurnitureFromPalette(data));
            }
        }

        private void PlaceFurnitureFromPalette(FurnitureData data)
        {
            if (homeManager == null) return;

            // Place in center of visible area
            Vector2Int centerPos = new Vector2Int(
                homeManager.GridSize.x / 2,
                homeManager.GridSize.y / 2
            );

            // Find first available position
            for (int radius = 0; radius < 5; radius++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {
                        Vector2Int testPos = centerPos + new Vector2Int(x, y);
                        if (homeManager.CanPlaceAt(testPos, data.size))
                        {
                            var item = homeManager.PlaceFurniture(data, testPos);
                            if (item != null)
                            {
                                homeManager.SelectFurniture(item);
                            }
                            return;
                        }
                    }
                }
            }

            Debug.Log("No space available to place furniture");
        }

        // Shop
        private void OpenShop()
        {
            shopPanel?.SetActive(true);
            PopulateShop();
        }

        private void CloseShop()
        {
            shopPanel?.SetActive(false);
        }

        private void PopulateShop()
        {
            if (homeManager == null || shopListContent == null || shopItemPrefab == null)
                return;

            // Clear existing items
            foreach (Transform child in shopListContent)
            {
                Destroy(child.gameObject);
            }

            foreach (var furnitureData in homeManager.FurnitureCatalog)
            {
                if (!furnitureData.isAvailable) continue;

                CreateShopItem(furnitureData);
            }
        }

        private void CreateShopItem(FurnitureData data)
        {
            GameObject item = Instantiate(shopItemPrefab, shopListContent);

            // Set up image
            var images = item.GetComponentsInChildren<Image>();
            foreach (var img in images)
            {
                if (img.gameObject.name == "Preview" && data.previewSprite != null)
                {
                    img.sprite = data.previewSprite;
                }
            }

            // Set up texts
            var texts = item.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var txt in texts)
            {
                if (txt.gameObject.name == "NameText")
                {
                    txt.text = data.furnitureName;
                }
                else if (txt.gameObject.name == "PriceText")
                {
                    txt.text = data.price.ToString("N0");
                }
            }

            // Set up buy button
            var button = item.GetComponentInChildren<Button>();
            if (button != null)
            {
                bool canAfford = coinManager != null && coinManager.CanAfford(data.price);
                button.interactable = canAfford;

                button.onClick.AddListener(() => BuyFurniture(data));
            }
        }

        private void BuyFurniture(FurnitureData data)
        {
            if (coinManager == null || !coinManager.SpendCoins(data.price))
            {
                Debug.Log("Cannot afford this furniture");
                return;
            }

            // Add to inventory (for now, just place it)
            CloseShop();
            EnterEditMode();
            PlaceFurnitureFromPalette(data);

            Debug.Log($"Purchased {data.furnitureName} for {data.price} coins");
        }

        private void OnDestroy()
        {
            if (homeManager != null)
            {
                homeManager.OnEditModeChanged.RemoveListener(OnEditModeChanged);
                homeManager.OnFurnitureSelected.RemoveListener(OnFurnitureSelected);
            }

            if (coinManager != null)
            {
                coinManager.OnCoinsChanged.RemoveListener(OnCoinsChanged);
            }
        }
    }
}
