using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace FocusHome
{
    public class HomeManager : MonoBehaviour
    {
        public static HomeManager Instance { get; private set; }

        [Header("Room Settings")]
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 8);
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector3 gridOrigin = Vector3.zero;

        [Header("Edit Mode")]
        [SerializeField] private bool isEditMode = false;
        [SerializeField] private FurnitureItem selectedFurniture;

        [Header("References")]
        [SerializeField] private Transform furnitureContainer;
        [SerializeField] private GameObject roomBackground;

        [Header("Furniture Catalog")]
        [SerializeField] private List<FurnitureData> furnitureCatalog;

        [Header("Placed Furniture")]
        [SerializeField] private List<FurnitureItem> placedFurniture = new List<FurnitureItem>();

        // Events
        public UnityEvent<bool> OnEditModeChanged;
        public UnityEvent<FurnitureItem> OnFurnitureSelected;
        public UnityEvent<FurnitureItem> OnFurniturePlaced;
        public UnityEvent<FurnitureItem> OnFurnitureRemoved;

        // Properties
        public bool IsEditMode => isEditMode;
        public FurnitureItem SelectedFurniture => selectedFurniture;
        public List<FurnitureData> FurnitureCatalog => furnitureCatalog;
        public Vector2Int RoomGridSize => gridSize;

        // Grid occupancy
        private bool[,] occupancyGrid;

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

            InitializeGrid();
        }

        private void Start()
        {
            LoadRoom();
        }

        private void InitializeGrid()
        {
            occupancyGrid = new bool[gridSize.x, gridSize.y];
        }

        public void ToggleEditMode()
        {
            SetEditMode(!isEditMode);
        }

        public void SetEditMode(bool editMode)
        {
            isEditMode = editMode;

            if (!isEditMode)
            {
                DeselectFurniture();
                SaveRoom();
            }

            OnEditModeChanged?.Invoke(isEditMode);
            Debug.Log($"Edit mode: {isEditMode}");
        }

        public void SelectFurniture(FurnitureItem furniture)
        {
            if (!isEditMode) return;

            // Deselect previous
            if (selectedFurniture != null)
            {
                selectedFurniture.Deselect();
            }

            selectedFurniture = furniture;

            if (selectedFurniture != null)
            {
                selectedFurniture.Select();
                OnFurnitureSelected?.Invoke(selectedFurniture);
            }
        }

        public void DeselectFurniture()
        {
            if (selectedFurniture != null)
            {
                selectedFurniture.Deselect();
                selectedFurniture = null;
            }
        }

        public void RotateSelectedFurniture()
        {
            if (selectedFurniture != null)
            {
                selectedFurniture.Rotate();
            }
        }

        public void RemoveSelectedFurniture()
        {
            if (selectedFurniture != null)
            {
                RemoveFurniture(selectedFurniture);
                selectedFurniture = null;
            }
        }

        public FurnitureItem PlaceFurniture(FurnitureData data, Vector2Int gridPos)
        {
            if (data == null || data.Prefab == null)
            {
                Debug.LogError("Invalid furniture data or missing prefab");
                return null;
            }

            if (!CanPlaceAt(gridPos, data.GridSize))
            {
                Debug.Log("Cannot place furniture at this position");
                return null;
            }

            // Instantiate furniture
            Vector3 worldPos = GridToWorld(gridPos);
            GameObject furnitureObj = Instantiate(data.Prefab, worldPos, Quaternion.identity, furnitureContainer);

            FurnitureItem item = furnitureObj.GetComponent<FurnitureItem>();
            if (item == null)
            {
                item = furnitureObj.AddComponent<FurnitureItem>();
            }

            item.Initialize(data, gridPos);
            item.SetPlaced(true);

            // Update occupancy
            SetOccupancy(gridPos, data.GridSize, true);

            placedFurniture.Add(item);
            OnFurniturePlaced?.Invoke(item);

            Debug.Log($"Placed {data.FurnitureName} at {gridPos}");
            return item;
        }

        public void RemoveFurniture(FurnitureItem item)
        {
            if (item == null) return;

            // Clear occupancy
            if (item.Data != null)
            {
                SetOccupancy(item.GridPosition, item.Data.GridSize, false);
            }

            placedFurniture.Remove(item);
            OnFurnitureRemoved?.Invoke(item);

            Destroy(item.gameObject);
            Debug.Log("Furniture removed");
        }

        public bool CanPlaceAt(Vector2Int gridPos, Vector2Int size, string excludeInstanceId = null)
        {
            int sizeX = size.x;
            int sizeY = size.y;

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    int checkX = gridPos.x + x;
                    int checkY = gridPos.y + y;

                    // Check bounds
                    if (checkX < 0 || checkX >= gridSize.x || checkY < 0 || checkY >= gridSize.y)
                    {
                        return false;
                    }

                    // Check occupancy (skip if it's the same item being moved)
                    if (occupancyGrid[checkX, checkY])
                    {
                        // Check if this cell is occupied by the item we're excluding
                        var occupyingItem = placedFurniture.FirstOrDefault(f =>
                            f.InstanceId != excludeInstanceId &&
                            IsWithinBounds(new Vector2Int(checkX, checkY), f.GridPosition, f.Data?.GridSize ?? Vector2Int.one));

                        if (occupyingItem != null)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool IsWithinBounds(Vector2Int point, Vector2Int origin, Vector2Int size)
        {
            return point.x >= origin.x && point.x < origin.x + size.x &&
                   point.y >= origin.y && point.y < origin.y + size.y;
        }

        private void SetOccupancy(Vector2Int gridPos, Vector2Int size, bool occupied)
        {
            int sizeX = size.x;
            int sizeY = size.y;

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    int posX = gridPos.x + x;
                    int posY = gridPos.y + y;

                    if (posX >= 0 && posX < gridSize.x && posY >= 0 && posY < gridSize.y)
                    {
                        occupancyGrid[posX, posY] = occupied;
                    }
                }
            }
        }

        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            return gridOrigin + new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0);
        }

        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector3 localPos = worldPos - gridOrigin;
            return new Vector2Int(
                Mathf.RoundToInt(localPos.x / cellSize),
                Mathf.RoundToInt(localPos.y / cellSize)
            );
        }

        // Save/Load
        public void SaveRoom()
        {
            var saveDataList = placedFurniture
                .Where(f => f != null)
                .Select(f => f.GetSaveData())
                .ToList();

            string json = JsonUtility.ToJson(new FurnitureSaveDataList { items = saveDataList });
            PlayerPrefs.SetString("RoomData", json);
            PlayerPrefs.Save();

            Debug.Log($"Room saved with {saveDataList.Count} furniture items");
        }

        public void LoadRoom()
        {
            string json = PlayerPrefs.GetString("RoomData", "");

            if (string.IsNullOrEmpty(json))
            {
                Debug.Log("No saved room data found");
                return;
            }

            try
            {
                var saveDataList = JsonUtility.FromJson<FurnitureSaveDataList>(json);

                foreach (var saveData in saveDataList.items)
                {
                    var furnitureData = furnitureCatalog.FirstOrDefault(f => f.Id == saveData.furnitureId);
                    if (furnitureData != null)
                    {
                        var item = PlaceFurniture(furnitureData, new Vector2Int(saveData.gridX, saveData.gridY));
                        if (item != null)
                        {
                            // Apply rotation
                            for (int i = 0; i < saveData.rotation / 90; i++)
                            {
                                item.Rotate();
                            }
                        }
                    }
                }

                Debug.Log($"Room loaded with {saveDataList.items.Count} furniture items");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load room: {e.Message}");
            }
        }

        // Debug visualization
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = new Color(0, 1, 0, 0.2f);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3 cellCenter = GridToWorld(new Vector2Int(x, y)) + new Vector3(cellSize / 2, cellSize / 2, 0);

                    if (occupancyGrid != null && occupancyGrid[x, y])
                    {
                        Gizmos.color = new Color(1, 0, 0, 0.3f);
                    }
                    else
                    {
                        Gizmos.color = new Color(0, 1, 0, 0.1f);
                    }

                    Gizmos.DrawCube(cellCenter, new Vector3(cellSize * 0.9f, cellSize * 0.9f, 0.1f));
                }
            }
        }
    }

    [System.Serializable]
    public class FurnitureSaveDataList
    {
        public List<FurnitureSaveData> items = new List<FurnitureSaveData>();
    }
}
