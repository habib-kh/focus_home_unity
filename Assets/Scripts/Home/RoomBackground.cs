using UnityEngine;

namespace FocusHome
{
    public class RoomBackground : MonoBehaviour
    {
        [Header("Room Components")]
        [SerializeField] private SpriteRenderer wallRenderer;
        [SerializeField] private SpriteRenderer floorRenderer;
        [SerializeField] private SpriteRenderer leftWallRenderer;
        [SerializeField] private SpriteRenderer rightWallRenderer;

        [Header("Default Colors")]
        [SerializeField] private Color wallColor = new Color(0.95f, 0.92f, 0.85f); // Cream
        [SerializeField] private Color floorColor = new Color(0.76f, 0.6f, 0.42f);  // Wood
        [SerializeField] private Color accentColor = new Color(0.55f, 0.45f, 0.35f); // Dark wood

        [Header("Room Themes")]
        [SerializeField] private RoomTheme[] themes;

        private int currentThemeIndex = 0;

        private void Start()
        {
            ApplyDefaultColors();
        }

        public void ApplyDefaultColors()
        {
            if (wallRenderer != null) wallRenderer.color = wallColor;
            if (floorRenderer != null) floorRenderer.color = floorColor;
            if (leftWallRenderer != null) leftWallRenderer.color = accentColor;
            if (rightWallRenderer != null) rightWallRenderer.color = accentColor;
        }

        public void ApplyTheme(int themeIndex)
        {
            if (themes == null || themeIndex < 0 || themeIndex >= themes.Length)
                return;

            currentThemeIndex = themeIndex;
            var theme = themes[themeIndex];

            if (wallRenderer != null)
            {
                if (theme.wallSprite != null)
                    wallRenderer.sprite = theme.wallSprite;
                wallRenderer.color = theme.wallColor;
            }

            if (floorRenderer != null)
            {
                if (theme.floorSprite != null)
                    floorRenderer.sprite = theme.floorSprite;
                floorRenderer.color = theme.floorColor;
            }
        }

        public void ApplyTheme(RoomTheme theme)
        {
            if (theme == null) return;

            if (wallRenderer != null)
            {
                if (theme.wallSprite != null)
                    wallRenderer.sprite = theme.wallSprite;
                wallRenderer.color = theme.wallColor;
            }

            if (floorRenderer != null)
            {
                if (theme.floorSprite != null)
                    floorRenderer.sprite = theme.floorSprite;
                floorRenderer.color = theme.floorColor;
            }
        }

        public int GetCurrentThemeIndex() => currentThemeIndex;

        public void NextTheme()
        {
            if (themes != null && themes.Length > 0)
            {
                currentThemeIndex = (currentThemeIndex + 1) % themes.Length;
                ApplyTheme(currentThemeIndex);
            }
        }

        // Generate a simple placeholder room background at runtime
        public void GeneratePlaceholderRoom()
        {
            // Create wall
            if (wallRenderer != null)
            {
                wallRenderer.sprite = CreateColoredSprite(256, 160, wallColor);
            }

            // Create floor
            if (floorRenderer != null)
            {
                floorRenderer.sprite = CreateColoredSprite(256, 96, floorColor);
            }
        }

        private Sprite CreateColoredSprite(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Point;

            return Sprite.Create(
                texture,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f),
                100f
            );
        }
    }

    [System.Serializable]
    public class RoomTheme
    {
        public string themeName;
        public Color wallColor = Color.white;
        public Color floorColor = Color.white;
        public Sprite wallSprite;
        public Sprite floorSprite;
        public int unlockPrice;
        public bool isUnlocked;
    }
}
