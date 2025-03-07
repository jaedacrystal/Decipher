# UIListAnimation API Documentation

## Table of Contents
1. [Introduction](#introduction)
2. [System Requirements](#system-requirements)
3. [Installation](#installation)
4. [Core Components](#core-components)
5. [API Reference](#api-reference)
6. [Code Examples](#code-examples)
7. [Best Practices](#best-practices)

## Introduction
UIListAnimation is a powerful and flexible Unity UI list animation system that makes it easy to add beautiful animation effects to UI elements in lists or grids. This animation system depends on DOTween.

## System Requirements
- Unity 2020.3 or higher
- DOTween/Pro (required)

## Installation
1. Import the UIListAnimationSystem folder into your Unity project
2. Ensure DOTween/Pro plugin is installed

## Core Components

### UIListAnimator
The main animation control component responsible for managing and playing UI list animations.

### AnimationConfig
Animation configuration file used to store animation properties and settings.

## API Reference

### UIListAnimator Class

#### Properties

| Property Name | Type | Description |
|--------------|------|-------------|
| Config | AnimationConfig | Animation configuration file |
| PlayOnEnable | bool | Whether to automatically play animation when enabled |
| AdditionalDelay | float | Additional delay time |
| PlayMode | PlayMode | Play mode (Normal/Grid) |

#### Methods

##### Play()
Start playing the animation.
```csharp
public void Play()
```

##### Stop()
Stop the current playing animation.
```csharp
public void Stop()
```

### AnimationConfig Class

#### Basic Settings

| Property | Type | Description |
|----------|------|-------------|
| StartDelay | float | Delay time before animation starts |
| ItemMode | ItemMode | Time mode (Interval/Duration) |
| ItemInterval | float | Time interval between items (Interval mode) |
| ItemDuration | float | Animation duration for items (Duration mode) |

#### Animation Property Configuration

Each animation property can include:

| Property | Type | Description |
|----------|------|-------------|
| Type | AnimationType | Animation type (Position/Scale/Rotation/CanvasGroup) |
| StartValue | Vector3/float | Start value |
| EndValue | Vector3/float | End value |
| Duration | float | Animation duration |
| EaseType | Ease | Easing type |
| CustomCurve | AnimationCurve | Custom animation curve |

## Code Examples

### Basic Usage
```csharp
// Get UIListAnimator component and play animation
var animator = gameObject.GetComponent<UIListAnimator>();
animator.Play();

// Set configuration and play
var animator = gameObject.GetComponent<UIListAnimator>();
var config = Resources.Load<AnimationConfig>("Path/To/Your/Config");
animator.Config = config;
animator.Play();

// Set delay and play
var animator = gameObject.GetComponent<UIListAnimator>();
animator.AdditionalDelay = 0.5f;
animator.Play();

// Stop animation
animator.Stop();
```

## Best Practices

### Recommended UI Structure
```
- List Container (UIListAnimator)
  - Item 1
    - Item Parent     <--- Target node for animation (requires CanvasGroup component)
      - Content1
      - Content2
      ...
  - Item 2
    - Item Parent     <--- Target node for animation (requires CanvasGroup component)
      - Content1
      - Content2
      ...
```

### Performance Optimization Tips
1. Set reasonable animation duration and intervals
2. Avoid too many simultaneous animations
3. Use object pooling for optimization when appropriate

### Important Notes
1. CanvasGroup animation type requires CanvasGroup component on target objects
2. Grid size modifications in editor will automatically adjust sequence numbers
3. Preview animation button is only available during runtime
4. `Play()` method can be called repeatedly, it will automatically stop previous animation and start new one
5. Ensure a valid animation configuration is set before calling `Play()` 