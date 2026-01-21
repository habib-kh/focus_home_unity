using UnityEngine;

namespace FocusHome
{
    /// <summary>
    /// Generates placeholder sprites for furniture items.
    /// Use this in Editor or at runtime to create colored placeholder sprites.
    /// </summary>
    public static class PlaceholderSpriteGenerator
    {
        public static Sprite CreatePlaceholderSprite(int width, int height, Color baseColor, string label = "")
        {
            Texture2D texture = new Texture2D(width, height);

            // Fill with base color
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = baseColor;
            }

            // Add border
            Color borderColor = baseColor * 0.7f;
            borderColor.a = 1f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Border pixels
                    if (x < 2 || x >= width - 2 || y < 2 || y >= height - 2)
                    {
                        pixels[y * width + x] = borderColor;
                    }
                }
            }

            // Add simple shadow on bottom and right
            Color shadowColor = new Color(0, 0, 0, 0.3f);
            for (int x = 4; x < width; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    int idx = y * width + x;
                    pixels[idx] = Color.Lerp(pixels[idx], shadowColor, 0.5f);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Point;

            return Sprite.Create(
                texture,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0f), // Pivot at bottom center for furniture
                100f
            );
        }

        // Predefined furniture colors
        public static Color SofaColor => new Color(0.6f, 0.4f, 0.3f);
        public static Color TableColor => new Color(0.55f, 0.35f, 0.2f);
        public static Color ChairColor => new Color(0.5f, 0.3f, 0.2f);
        public static Color LampColor => new Color(0.9f, 0.85f, 0.7f);
        public static Color PlantColor => new Color(0.3f, 0.6f, 0.3f);
        public static Color RugColor => new Color(0.7f, 0.5f, 0.4f);
        public static Color ShelfColor => new Color(0.45f, 0.3f, 0.2f);
        public static Color BedColor => new Color(0.8f, 0.75f, 0.9f);
        public static Color DeskColor => new Color(0.5f, 0.35f, 0.25f);
        public static Color CabinetColor => new Color(0.4f, 0.28f, 0.18f);
    }
}
