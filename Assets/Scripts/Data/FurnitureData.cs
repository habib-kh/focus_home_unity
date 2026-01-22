using UnityEngine;

namespace FocusHome
{
    public enum FurnitureCategory
    {
        Sofa,
        Chair,
        Table,
        Bed,
        Storage,
        Lamp,
        Plant,
        Decoration,
        Rug,
        WallArt
    }

    [CreateAssetMenu(fileName = "New Furniture", menuName = "FocusHome/Furniture Data")]
    public class FurnitureData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string id;
        [SerializeField] private string furnitureName;
        [TextArea] [SerializeField] private string description;
        [SerializeField] private FurnitureCategory category;

        [Header("Visuals")]
        [SerializeField] private Sprite sprite;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Vector2Int gridSize = Vector2Int.one;

        [Header("Shop")]
        [SerializeField] private int price = 100;
        [SerializeField] private bool isAvailable = true;
        [SerializeField] private bool isSpecial = false;

        [Header("Unlock")]
        [SerializeField] private bool requiresUnlock = false;
        [SerializeField] private int unlockAtFocusHours = 0;
        [SerializeField] private string unlockAchievementId;

        // Public properties for access
        public string Id => id;
        public string FurnitureName => furnitureName;
        public string Description => description;
        public FurnitureCategory Category => category;
        public Sprite Sprite => sprite;
        public GameObject Prefab => prefab;
        public Vector2Int GridSize => gridSize;
        public int Price => price;
        public bool IsAvailable => isAvailable;
        public bool IsSpecial => isSpecial;
        public bool RequiresUnlock => requiresUnlock;
        public int UnlockAtFocusHours => unlockAtFocusHours;
        public string UnlockAchievementId => unlockAchievementId;
    }
}
