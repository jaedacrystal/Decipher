# UI List Animation Demo Guide

## Scene Overview

This demo scene demonstrates a 4x4 grid layout list animation effect. The scene is located at `Plugins/UIListAnimation/Demo/Scenes/Grid_4x4.unity`.

### Scene Structure

The scene contains the following components:

- Canvas: UI root node
  - Container: List container
    - 16 Item prefab instances (4x4 grid layout)

### Animation Configurations

The `ListAnimationConfig` folder contains several preset animation configurations:

1. Position.asset
   - Up-sliding animation
   - Start position: (0, -50, 0)
   - End position: (0, 0, 0)
   - Duration: 0.3 seconds

2. Position_L.asset 
   - Left-to-right sliding + fade-in animation
   - Start position: (-200, 0, 0)
   - End position: (0, 0, 0)
   - Duration: 0.3 seconds

3. Position_R.asset
   - Right-to-left sliding + fade-in animation  
   - Start position: (200, 0, 0)
   - End position: (0, 0, 0)
   - Duration: 0.3 seconds

4. Scale.asset
   - Scale + fade-in animation
   - Start scale: (0.5, 0.5, 0.5)
   - End scale: (1, 1, 1)
   - Duration: 0.3 seconds

### Runtime Effect

1. When the scene runs, the Container component automatically plays the selected animation configuration
2. Each Item plays animation sequentially according to the grid order
3. Different animation configurations can be switched through the Container component's Inspector panel
4. Animation parameters include:
   - Play mode: Auto/Manual
   - Grid rows: 4
   - Grid columns: 4
   - Item interval: 0.05 seconds
   - Animation duration: 0.3 seconds

### How to Use

1. Open Grid_4x4 scene
2. Select the Container game object
3. In the Inspector, you can:
   - Select animation configuration file
   - Adjust grid parameters
   - Set auto-play option
   - Modify animation delay time
4. Run the scene to see the list animation effect 