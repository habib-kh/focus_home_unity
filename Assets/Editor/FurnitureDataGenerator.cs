#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace FocusHome.Editor
{
    public class FurnitureDataGenerator : EditorWindow
    {
        [MenuItem("FocusHome/Generate Furniture Data")]
        public static void ShowWindow()
        {
            GetWindow<FurnitureDataGenerator>("Furniture Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Furniture Data Generator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Generate All Default Furniture"))
            {
                GenerateDefaultFurniture();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Generate Placeholder Sprites"))
            {
                GeneratePlaceholderSprites();
            }

            GUILayout.Space(20);
            GUILayout.Label("Individual Categories", EditorStyles.boldLabel);

            if (GUILayout.Button("Generate Sofas")) GenerateCategory(FurnitureCategory.Sofa);
            if (GUILayout.Button("Generate Tables")) GenerateCategory(FurnitureCategory.Table);
            if (GUILayout.Button("Generate Chairs")) GenerateCategory(FurnitureCategory.Chair);
            if (GUILayout.Button("Generate Lamps")) GenerateCategory(FurnitureCategory.Lamp);
            if (GUILayout.Button("Generate Plants")) GenerateCategory(FurnitureCategory.Plant);
            if (GUILayout.Button("Generate Rugs")) GenerateCategory(FurnitureCategory.Rug);
            if (GUILayout.Button("Generate Decorations")) GenerateCategory(FurnitureCategory.Decoration);
        }

        private void GenerateDefaultFurniture()
        {
            string dataPath = "Assets/Data/Furniture";
            EnsureDirectory(dataPath);

            // Sofas
            CreateFurnitureData(dataPath, "sofa_classic", "Classic Sofa", FurnitureCategory.Sofa, 150, new Vector2Int(3, 2));
            CreateFurnitureData(dataPath, "sofa_modern", "Modern Sofa", FurnitureCategory.Sofa, 200, new Vector2Int(3, 2));
            CreateFurnitureData(dataPath, "sofa_corner", "Corner Sofa", FurnitureCategory.Sofa, 350, new Vector2Int(4, 3));

            // Tables
            CreateFurnitureData(dataPath, "table_coffee", "Coffee Table", FurnitureCategory.Table, 80, new Vector2Int(2, 1));
            CreateFurnitureData(dataPath, "table_dining", "Dining Table", FurnitureCategory.Table, 200, new Vector2Int(3, 2));
            CreateFurnitureData(dataPath, "table_side", "Side Table", FurnitureCategory.Table, 50, new Vector2Int(1, 1));

            // Chairs
            CreateFurnitureData(dataPath, "chair_dining", "Dining Chair", FurnitureCategory.Chair, 60, new Vector2Int(1, 1));
            CreateFurnitureData(dataPath, "chair_armchair", "Armchair", FurnitureCategory.Chair, 120, new Vector2Int(2, 2));
            CreateFurnitureData(dataPath, "chair_office", "Office Chair", FurnitureCategory.Chair, 100, new Vector2Int(1, 1));

            // Lamps
            CreateFurnitureData(dataPath, "lamp_floor", "Floor Lamp", FurnitureCategory.Lamp, 70, new Vector2Int(1, 1));
            CreateFurnitureData(dataPath, "lamp_table", "Table Lamp", FurnitureCategory.Lamp, 45, new Vector2Int(1, 1));
            CreateFurnitureData(dataPath, "lamp_chandelier", "Chandelier", FurnitureCategory.Lamp, 300, new Vector2Int(2, 2));

            // Plants
            CreateFurnitureData(dataPath, "plant_small", "Small Plant", FurnitureCategory.Plant, 30, new Vector2Int(1, 1));
            CreateFurnitureData(dataPath, "plant_large", "Large Plant", FurnitureCategory.Plant, 80, new Vector2Int(1, 2));
            CreateFurnitureData(dataPath, "plant_tree", "Indoor Tree", FurnitureCategory.Plant, 150, new Vector2Int(2, 3));

            // Rugs
            CreateFurnitureData(dataPath, "rug_basic", "Basic Rug", FurnitureCategory.Rug, 40, new Vector2Int(2, 2));
            CreateFurnitureData(dataPath, "rug_large", "Large Rug", FurnitureCategory.Rug, 100, new Vector2Int(4, 3));
            CreateFurnitureData(dataPath, "rug_runner", "Runner Rug", FurnitureCategory.Rug, 60, new Vector2Int(4, 1));

            // Decorations
            CreateFurnitureData(dataPath, "deco_painting", "Wall Painting", FurnitureCategory.Decoration, 75, new Vector2Int(2, 2));
            CreateFurnitureData(dataPath, "deco_vase", "Decorative Vase", FurnitureCategory.Decoration, 35, new Vector2Int(1, 1));
            CreateFurnitureData(dataPath, "deco_bookshelf", "Bookshelf", FurnitureCategory.Decoration, 180, new Vector2Int(2, 3));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Generated all default furniture data!");
        }

        private void GenerateCategory(FurnitureCategory category)
        {
            string dataPath = "Assets/Data/Furniture";
            EnsureDirectory(dataPath);

            // Generate sample items for the category
            string prefix = category.ToString().ToLower();
            CreateFurnitureData(dataPath, $"{prefix}_1", $"{category} Item 1", category, 50, new Vector2Int(1, 1));
            CreateFurnitureData(dataPath, $"{prefix}_2", $"{category} Item 2", category, 100, new Vector2Int(2, 2));
            CreateFurnitureData(dataPath, $"{prefix}_3", $"{category} Item 3", category, 150, new Vector2Int(2, 3));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Generated {category} furniture data!");
        }

        private void CreateFurnitureData(string path, string id, string name, FurnitureCategory category, int price, Vector2Int size)
        {
            string assetPath = $"{path}/{id}.asset";

            // Check if already exists
            var existing = AssetDatabase.LoadAssetAtPath<FurnitureData>(assetPath);
            if (existing != null)
            {
                Debug.Log($"Furniture {id} already exists, skipping...");
                return;
            }

            var data = ScriptableObject.CreateInstance<FurnitureData>();

            // Use reflection or serialized object to set private fields
            var so = new SerializedObject(data);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("furnitureName").stringValue = name;
            so.FindProperty("category").enumValueIndex = (int)category;
            so.FindProperty("price").intValue = price;
            so.FindProperty("gridSize").vector2IntValue = size;
            so.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(data, assetPath);
            Debug.Log($"Created furniture: {name}");
        }

        private void GeneratePlaceholderSprites()
        {
            string spritePath = "Assets/Art/Sprites/Furniture";
            EnsureDirectory(spritePath);

            // Generate placeholder textures for each furniture type
            GeneratePlaceholderTexture(spritePath, "sofa", 192, 96, new Color(0.6f, 0.4f, 0.3f));
            GeneratePlaceholderTexture(spritePath, "table", 128, 64, new Color(0.55f, 0.35f, 0.2f));
            GeneratePlaceholderTexture(spritePath, "chair", 64, 64, new Color(0.5f, 0.3f, 0.2f));
            GeneratePlaceholderTexture(spritePath, "lamp", 32, 96, new Color(0.9f, 0.85f, 0.7f));
            GeneratePlaceholderTexture(spritePath, "plant", 48, 80, new Color(0.3f, 0.6f, 0.3f));
            GeneratePlaceholderTexture(spritePath, "rug", 128, 96, new Color(0.7f, 0.5f, 0.4f));
            GeneratePlaceholderTexture(spritePath, "decoration", 64, 64, new Color(0.8f, 0.7f, 0.6f));

            AssetDatabase.Refresh();
            Debug.Log("Generated placeholder sprites!");
        }

        private void GeneratePlaceholderTexture(string path, string name, int width, int height, Color baseColor)
        {
            Texture2D texture = new Texture2D(width, height);

            // Fill with gradient
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float gradientY = (float)y / height;
                    Color pixelColor = Color.Lerp(baseColor * 0.8f, baseColor, gradientY);

                    // Add border
                    if (x < 2 || x >= width - 2 || y < 2 || y >= height - 2)
                    {
                        pixelColor = baseColor * 0.5f;
                    }

                    texture.SetPixel(x, y, pixelColor);
                }
            }

            texture.Apply();

            // Save as PNG
            byte[] pngData = texture.EncodeToPNG();
            string filePath = $"{path}/{name}_placeholder.png";
            File.WriteAllBytes(filePath, pngData);

            DestroyImmediate(texture);
        }

        private void EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string[] parts = path.Split('/');
                string current = parts[0];

                for (int i = 1; i < parts.Length; i++)
                {
                    string next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                    {
                        AssetDatabase.CreateFolder(current, parts[i]);
                    }
                    current = next;
                }
            }
        }
    }
}
#endif
