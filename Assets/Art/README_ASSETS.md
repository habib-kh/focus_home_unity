# FocusHome Art Assets

## Getting Isometric Furniture Sprites

For Homescapes-style graphics, you need pre-rendered isometric 2D sprites. Here are recommended sources:

### Free Asset Sources

1. **Kenney.nl** (https://kenney.nl)
   - Free game assets, including furniture packs
   - Look for "Furniture Kit" or "House Kit"

2. **OpenGameArt.org** (https://opengameart.org)
   - Search for "isometric furniture" or "room furniture"
   - Many free CC0 licensed assets

3. **itch.io** (https://itch.io/game-assets/free)
   - Search for "isometric" or "furniture"
   - Many free and paid options

### Paid Asset Sources (Higher Quality)

1. **Unity Asset Store** (https://assetstore.unity.com)
   - Search for "isometric furniture" or "2D room"
   - Recommended packs:
     - "Isometric Furniture Pack"
     - "2D Room Interior Assets"

2. **GraphicRiver** (https://graphicriver.net)
   - High quality pre-rendered sprites

3. **GameDevMarket** (https://www.gamedevmarket.net)
   - Game-specific asset packs

### AI-Generated Sprites

You can generate sprites using AI tools:

1. **Midjourney** - Prompt: "isometric furniture sprite, [item name], game asset, transparent background"
2. **DALL-E** - Similar prompts
3. **Stable Diffusion** with ControlNet for consistent style

### Creating Your Own

Tools for creating isometric sprites:
- **Blender** (free) - Create 3D models and render to 2D
- **Aseprite** (paid) - Pixel art with isometric guides
- **Adobe Illustrator** - Vector isometric art

## Folder Structure

Place your sprites in:
```
Assets/
├── Art/
│   ├── Sprites/
│   │   ├── Furniture/
│   │   │   ├── Sofas/
│   │   │   ├── Tables/
│   │   │   ├── Chairs/
│   │   │   ├── Lamps/
│   │   │   ├── Plants/
│   │   │   ├── Rugs/
│   │   │   └── Decorations/
│   │   ├── Rooms/
│   │   │   ├── Backgrounds/
│   │   │   ├── Floors/
│   │   │   └── Walls/
│   │   └── UI/
│   └── Materials/
```

## Sprite Import Settings

For best results, configure imported sprites:

1. **Texture Type**: Sprite (2D and UI)
2. **Sprite Mode**: Single
3. **Pixels Per Unit**: 100 (adjust based on your sprites)
4. **Filter Mode**: Point (for pixel art) or Bilinear (for smooth art)
5. **Compression**: None (for best quality) or Normal

## Using Placeholder Sprites

Until you have real assets:

1. Open Unity Editor
2. Go to menu: **FocusHome > Generate Furniture Data**
3. Click "Generate Placeholder Sprites"
4. Colored placeholder images will be created in Assets/Art/Sprites/Furniture/

## Sprite Specifications

For consistent look:
- **Standard furniture**: 64-256 pixels wide
- **Pivot point**: Bottom center (for floor placement)
- **Transparent background**: Required
- **Color palette**: Consistent warm tones for Homescapes style
