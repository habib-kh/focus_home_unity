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
        public string furnitureId;
        public string furnitureName;
        [TextArea] public string description;
        public FurnitureCategory category;

        [Header("Visuals")]
        public Sprite previewSprite;
        public GameObject prefab;
        public Vector2 size = Vector2.one; // Grid size

        [Header("Shop")]
        public int price = 100;
        public bool isAvailable = true;
        public bool isSpecial = false;

        [Header("Unlock")]
        public bool requiresUnlock = false;
        public int unlockAtFocusHours = 0;
        public string unlockAchievementId;
    }
}
