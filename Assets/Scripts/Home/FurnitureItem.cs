using UnityEngine;
using UnityEngine.EventSystems;

namespace FocusHome
{
    public class FurnitureItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Data")]
        [SerializeField] private FurnitureData furnitureData;
        [SerializeField] private string instanceId;

        [Header("Placement")]
        [SerializeField] private Vector2Int gridPosition;
        [SerializeField] private int rotation = 0; // 0, 90, 180, 270

        [Header("State")]
        [SerializeField] private bool isPlaced = false;
        [SerializeField] private bool isSelected = false;
        [SerializeField] private bool isDragging = false;

        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject selectionIndicator;
        [SerializeField] private Collider2D itemCollider;

        // Properties
        public FurnitureData Data => furnitureData;
        public string InstanceId => instanceId;
        public Vector2Int GridPosition => gridPosition;
        public int Rotation => rotation;
        public bool IsPlaced => isPlaced;
        public bool IsSelected => isSelected;

        private Vector3 dragOffset;
        private Vector3 originalPosition;
        private HomeManager homeManager;

        private void Awake()
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                instanceId = System.Guid.NewGuid().ToString();
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(false);
            }
        }

        private void Start()
        {
            homeManager = HomeManager.Instance;
        }

        public void Initialize(FurnitureData data, Vector2Int position)
        {
            furnitureData = data;
            gridPosition = position;
            instanceId = System.Guid.NewGuid().ToString();

            if (spriteRenderer != null && data.previewSprite != null)
            {
                spriteRenderer.sprite = data.previewSprite;
            }

            UpdateSortingOrder();
        }

        public void SetGridPosition(Vector2Int position)
        {
            gridPosition = position;
            UpdateSortingOrder();
        }

        public void SetPlaced(bool placed)
        {
            isPlaced = placed;
        }

        public void Select()
        {
            isSelected = true;
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(true);
            }

            // Visual feedback
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            }
        }

        public void Deselect()
        {
            isSelected = false;
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(false);
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
        }

        public void Rotate()
        {
            rotation = (rotation + 90) % 360;
            transform.rotation = Quaternion.Euler(0, 0, -rotation);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isDragging && homeManager != null && homeManager.IsEditMode)
            {
                homeManager.SelectFurniture(this);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (homeManager == null || !homeManager.IsEditMode) return;

            isDragging = true;
            originalPosition = transform.position;

            // Calculate offset from click point to object center
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPoint.z = transform.position.z;
            dragOffset = transform.position - worldPoint;

            // Visual feedback
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.7f);
            }

            // Bring to front while dragging
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 1000;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPoint.z = transform.position.z;
            transform.position = worldPoint + dragOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging) return;

            isDragging = false;

            // Reset visual
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }

            // Snap to grid or validate position
            if (homeManager != null)
            {
                Vector3 worldPos = transform.position;
                Vector2Int newGridPos = homeManager.WorldToGrid(worldPos);

                if (homeManager.CanPlaceAt(newGridPos, furnitureData.size, instanceId))
                {
                    gridPosition = newGridPos;
                    transform.position = homeManager.GridToWorld(newGridPos);
                }
                else
                {
                    // Return to original position
                    transform.position = originalPosition;
                }
            }

            UpdateSortingOrder();
        }

        private void UpdateSortingOrder()
        {
            // Items lower on screen (higher Y in grid) should render on top
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = gridPosition.y * 10;
            }
        }

        // Save data for serialization
        public FurnitureSaveData GetSaveData()
        {
            return new FurnitureSaveData
            {
                instanceId = instanceId,
                furnitureId = furnitureData?.furnitureId ?? "",
                gridX = gridPosition.x,
                gridY = gridPosition.y,
                rotation = rotation
            };
        }
    }

    [System.Serializable]
    public class FurnitureSaveData
    {
        public string instanceId;
        public string furnitureId;
        public int gridX;
        public int gridY;
        public int rotation;
    }
}
