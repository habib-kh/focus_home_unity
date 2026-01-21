# FocusHome Unity

A focus timer app with home decoration rewards, built in Unity for better 3D graphics (Homescapes-style).

## Setup Instructions

### Prerequisites
- Unity 2022.3 LTS or newer (recommended: Unity 6)
- Unity Hub installed

### Creating the Project

1. **Open Unity Hub**
2. **Create New Project:**
   - Click "New Project"
   - Select "2D (URP)" or "2D" template
   - Name: `focus_home_unity`
   - Location: `C:\Users\th-h.khademhosseini\Projects\`
   - Click "Create Project"

3. **After Unity Opens:**
   - The `Assets` folder already contains scripts
   - Unity will automatically import them

### Required Packages

Install via Window > Package Manager:

1. **TextMeshPro** (usually included)
2. **2D Sprite** (for sprite rendering)
3. **2D Tilemap** (optional, for grid-based rooms)

### Setting Up Scenes

Create these scenes (File > New Scene, then save):

1. **TimerScene** - Focus timer
2. **HomeScene** - Room decoration (main scene)
3. **ShopScene** - Furniture shop
4. **ProfileScene** - User profile/stats

### Scene Setup

#### HomeScene Setup:
1. Create empty GameObject "Managers"
   - Add `GameManager.cs`
   - Add `CoinManager.cs`
   - Add `HomeManager.cs`

2. Create empty GameObject "Room"
   - Add your room background sprite
   - This is where furniture will be placed

3. Create Canvas for UI
   - Add `HomeUI.cs` to Canvas
   - Set up UI elements (see UI section below)

#### TimerScene Setup:
1. Create Canvas for UI
2. Add `TimerUI.cs`
3. Create `FocusTimer` GameObject with `FocusTimer.cs`

### Adding Furniture

1. **Import 2D Assets:**
   - Download isometric furniture sprites
   - Place in `Assets/Textures/Furniture/`

2. **Create Furniture Data:**
   - Right-click in Project > Create > FocusHome > Furniture Data
   - Fill in: name, sprite, price, category, size
   - Assign preview sprite and prefab

3. **Create Furniture Prefab:**
   - Create new GameObject with SpriteRenderer
   - Add `FurnitureItem.cs` component
   - Add Collider2D for interaction
   - Save as Prefab in `Assets/Prefabs/Furniture/`

### Recommended Free Assets

Search Unity Asset Store for:
- "Isometric furniture"
- "2D interior"
- "Homescapes style"

Free options:
- Kenney.nl game assets
- OpenGameArt.org
- itch.io game assets

### Project Structure

```
Assets/
├── Scripts/
│   ├── Managers/       # GameManager, CoinManager
│   ├── Timer/          # FocusTimer
│   ├── Home/           # HomeManager, FurnitureItem
│   ├── Data/           # ScriptableObjects
│   └── UI/             # UI Controllers
├── Scenes/
├── Prefabs/
│   ├── Furniture/
│   └── UI/
├── Textures/
│   ├── Furniture/
│   ├── Rooms/
│   └── UI/
├── Materials/
├── Animations/
└── Audio/
```

### Building for Mobile

1. **Android:**
   - File > Build Settings
   - Select Android
   - Click "Switch Platform"
   - Player Settings > Package Name: `com.yourname.focushome`
   - Build

2. **iOS:**
   - Requires Mac with Xcode
   - Similar process to Android

### Key Features

- **Focus Timer:** 15/25/45/60 min presets with penalty system
- **Coin System:** Earn coins by focusing
- **Room Editor:** Drag & drop furniture placement
- **Shop:** Buy new furniture with coins
- **Persistence:** Saves progress locally

### Scripts Overview

| Script | Purpose |
|--------|---------|
| `GameManager.cs` | Main game controller, saves/loads data |
| `CoinManager.cs` | Manages currency |
| `FocusTimer.cs` | Timer logic with app lifecycle detection |
| `HomeManager.cs` | Room/furniture management |
| `FurnitureItem.cs` | Individual furniture behavior |
| `FurnitureData.cs` | ScriptableObject for furniture stats |
| `TimerUI.cs` | Timer screen UI |
| `HomeUI.cs` | Home editor UI |

### Tips for Homescapes-Style Graphics

1. **Use 2D Isometric sprites** (not real 3D)
2. **Layer sprites by Y position** (items lower on screen render on top)
3. **Add shadows** as separate sprites below furniture
4. **Use warm color palette** (cream walls, honey wood floors)
5. **Add decorative details** (cushions, books, plants)

## Next Steps

1. Open Unity and create the project
2. Import the scripts (they're already in Assets/)
3. Download isometric furniture assets
4. Create ScriptableObjects for each furniture piece
5. Set up the scenes
6. Test on device!
