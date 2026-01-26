# Scene Layers for Unity

Layer management system for Unity scenes. Organize, hide, lock, and manage scene objects with an intuitive layer-based workflow.

## Features

### Core Layer Management
- **Organize objects into layers** - Group related GameObjects (UI, environment, lighting, VFX, etc.)
- **Hide/Lock layers** - Toggle visibility and picking for entire layers at once
- **Drag & drop assignment** - Drag objects from hierarchy directly onto layers
- **Color-coded hierarchy** - See which layers objects belong to at a glance
- **Search & filter** - Quickly find layers and objects by name

### Advanced Features
- **Layer Views** - Save and restore layer visibility states (like Photoshop layer comps)
- **Auto-assign rules** - Automatically assign objects to layers based on components
- **Layer presets** - Save and load entire layer setups between scenes
- **Drag reorder** - Reorder both layers and objects within layers
- **Multi-layer objects** - Objects can belong to multiple layers simultaneously
- **Per-scene databases** - Each scene has its own independent layer setup

### Compact Interfaces
- **Views Panel** - Dockable panel showing only layer view buttons
- **Floating Toolbar** - Minimal floating window for quick view switching

## Installation

1. Import the package into your Unity project
2. The tool will automatically create the necessary folder structure

## Quick Start

### Opening the Window
- **Window > Scene Layers > Layer Manager** to open the main window
- **Window > Scene Layers > Views Panel** for a compact dockable views panel
- **Window > Scene Layers > Floating Toolbar** for a minimal floating view switcher
- **Window > Scene Layers > Options** for settings

### Creating Layers
1. Click the **"New Layer"** button at the bottom of the layer list
2. Enter a name and choose a color
3. Click **Create**

### Adding Objects to Layers
**Method 1: Drag & Drop**
- Drag GameObjects from the Hierarchy directly onto a layer header

**Method 2: Auto-assign Rules**
- Click the ⚙ (gear) icon on a layer
- Add component types (e.g., "Light", "Camera")
- Click "Scan Scene Now" to auto-assign matching objects

### Layer Views (Visibility Presets)
1. Set up your layers' visibility states as desired
2. Click the **Save** icon (💾) in the Layer Views section
3. Give the view a name and color
4. Click the view button to restore that visibility state anytime

### Layer Presets
1. Click the preset icon (⚙) in the top-right
2. Save your current setup as a preset
3. Load presets in other scenes to quickly replicate your layer structure

## Interface Guide

### Top Section: Layer Views
- **View buttons** - Click to apply a saved visibility state
- **Settings icon (⚙)** - Manage saved views
- **Save icon (💾)** - Save current visibility state as a new view

### Middle Section: Layers List
Each layer header shows:
- **👁 Eye icon** - Toggle layer visibility
- **🔒 Lock icon** - Toggle picking (selection) for layer
- **Layer name** - Double-click to rename
- **Object count** - Number of objects in layer
- **☆ Select All** - Select all objects in layer
- **⚙ Rules** - Configure auto-assign rules
- **✕ Delete** - Delete the layer

### Layer Objects (when expanded)
- **👁 Show/Hide** - Toggle object visibility
- **🔒 Lock/Unlock** - Toggle object picking
- **Object name** - Click to ping, double-click to select
- **Drag handle** - Drag to reorder or move between layers

### Views Panel
A compact, dockable panel that displays only the layer view buttons. Ideal for docking above the Hierarchy window for quick access without taking up much space.

**Features:**
- Shows current scene name in toolbar
- **Manage** button opens the main Layer Manager window
- Automatically loads views for the current scene
- View buttons resize and wrap to multiple rows based on panel width
- Buttons are distributed evenly across rows (e.g., 6 buttons = 3+3, not 4+2)

**Usage:**
1. Open via **Window > Scene Layers > Views Panel**
2. Dock it above your Hierarchy for quick access
3. Click any view button to apply that visibility state

### Floating Toolbar
A minimal floating window for rapid view switching, similar to Blender's floating toolbars.

**Features:**
- Chromeless design with minimal visual footprint
- **Drag handle** at top (dotted grip) - drag to reposition
- **Resize handle** at bottom-right (diagonal grip) - drag to resize
- **Close button** (✕) in top-right corner
- Buttons wrap to multiple rows when window is narrowed
- Remembers position and size between sessions
- Height automatically adjusts to fit all buttons

**Usage:**
1. Open via **Window > Scene Layers > Floating Toolbar**
2. Drag from the top grip to position anywhere on screen
3. Resize from the bottom-right corner as needed
4. Click any view button to apply that visibility state
5. Click ✕ to close

**Tips:**
- Position it near your Scene View for easy access while working
- Resize horizontally to control button layout (wider = more buttons per row)
- The toolbar stays on top of other windows

## Keyboard Shortcuts
- **Enter** - Confirm layer/view rename
- **Escape** - Cancel rename
- **Double-click** layer name - Start rename
- **Double-click** view name - Start rename

## Options

Access via **Window > Scene Layers > Options**

- **Layer Colors** - Show/hide colored backgrounds in hierarchy
- **Clicking Object Row** - Choose between Ping or Select behavior
- **Multi-Layer Objects > Color Split Direction** - Vertical or horizontal stripes for objects in multiple layers

## Tips & Best Practices

### Organization Strategies
- **By type**: Lighting, Audio, VFX, Gameplay, UI
- **By system**: Player, Enemies, Environment, Collectibles
- **By state**: Always Visible, Debug Only, Optional Content
- **By LOD**: High Detail, Medium Detail, Low Detail

### Workflow Suggestions
1. **Start with broad categories** - Create 4-6 main layers first
2. **Use auto-rules for common types** - Lights, Cameras, Audio Sources
3. **Create views for different modes** - "Level Design Mode", "Lighting Mode", "Gameplay Mode"
4. **Use presets for consistency** - Create a preset for your team's standard layers
5. **Use the Views Panel** - Dock it above your Hierarchy for quick view switching
6. **Use the Floating Toolbar** - Keep it near your Scene View for rapid access

### Choosing an Interface
- **Main Layer Manager** - Full control over layers, objects, views, and settings
- **Views Panel** - When you need quick view switching in a docked panel
- **Floating Toolbar** - When you want minimal UI that floats over your workspace

### Performance Tips
- Layer operations are fast and lightweight
- Visibility changes use Unity's built-in SceneVisibilityManager
- Layer membership is stored per-scene, not on GameObjects
- No runtime performance impact (editor-only tool)

## Technical Details

### Scene-Specific Databases
Each scene gets its own `[SceneName]_SceneLayers.asset` file stored alongside the scene. This means:
- Layer setups are per-scene
- No conflicts when switching scenes
- Easy to version control
- Can be shared with team members

### Data Storage
- **Layer membership**: Stored using GlobalObjectId (persists through scene reloads)
- **Object order**: Stored in EditorPrefs (survives domain reloads)
- **UI state**: Stored in EditorPrefs per-scene (foldouts, selected layer, etc.)
- **Floating Toolbar position**: Stored in EditorPrefs (persists between sessions)

### Unity Version Support
- **Minimum**: Unity 2021.3 LTS
- **Recommended**: Unity 2022.3 LTS or later
- **Tested**: 2021 LTS, 2022 LTS, 2023 LTS, 2024+

### Render Pipeline Support
- Built-in Render Pipeline ✔
- Universal Render Pipeline (URP) ✔
- High Definition Render Pipeline (HDRP) ✔

## Troubleshooting

### Objects not appearing in layers after scene reload
- This is normal - the tool uses GlobalObjectId which updates after scene load
- The objects will reappear once Unity processes the scene
- If objects are truly missing, re-assign them

### Layer colors not showing in hierarchy
- Check **Window > Scene Layers > Options**
- Enable "Layer Colors"

### Can't select objects in scene
- Check if the layer is locked (🔒 icon should be open)
- Unlock the layer to enable picking

### Database file missing
- The database is created automatically when you create your first layer
- It's stored next to your scene file: `[SceneName]_SceneLayers.asset`
- Don't delete this file or your layer data will be lost

### Preset not applying correctly
- Presets work by matching layer names
- If layers have been renamed, the preset may not apply fully
- Consider creating a new preset if layer structure has changed significantly

### Views Panel or Floating Toolbar showing "No views yet"
- Create layer views in the main Layer Manager window first
- Use the **Manage** button in the Views Panel to open the main window
- Save a view using the 💾 icon in the Layer Views section

### Floating Toolbar position reset
- Position is saved when the toolbar is closed
- If Unity crashes, the last saved position may be lost
- Reopen the toolbar and reposition as needed

## Limitations

- **Editor-only**: This is an editor tool and has no runtime functionality
- **Scene objects only**: Prefab assets cannot be permanently assigned to layers (only their instances)
- **GlobalObjectId dependent**: Object references rely on Unity's GlobalObjectId system
- **No nested layers**: Layers are flat, not hierarchical

## Support & Feedback

For issues, feature requests, or questions:
- Check the documentation first
- Review the troubleshooting section
- Contact: cheekychopslabs@gmail.com

## Changelog

### Version 1.2.0

- Added camera position saving to Layer Views - save and restore Scene View camera position, rotation, and zoom
- Camera toggle button in View Manager for quick camera data management
- Visual indicators show which views have camera data saved
- Virtual scrolling implementation for massive performance boost with large layers (1000+ objects)
- Fixed object reordering bug on first drag after loading preset or switching scenes
- Fixed erratic drag behavior when dragging objects at different positions in large layers
- Improved order synchronization system for consistent object ordering
- Enhanced drag-and-drop reliability across all layer sizes

### Version 1.1.0
- Added Views Panel - compact dockable panel for quick view switching
- Added Floating Toolbar - minimal floating window for rapid view access
- Improved button layout - even distribution across rows (3+3 instead of 4+2)
- Views can be applied from any interface without opening the main window

### Version 1.0.0 (Initial Release)
- Complete layer management system
- Layer views (visibility presets)
- Auto-assign rules with component filtering
- Layer presets for cross-scene sharing
- Drag & drop support for objects and layers
- Color-coded hierarchy highlighting
- Per-scene layer databases
- Multi-layer object support
- Comprehensive search and filtering

## License

© Copyright 2025 Cheeky Chops Labs