# GitHub Copilot Instructions for RimWorld Modding Project

## Mod Overview and Purpose

### Overview
The purpose of this mod is to enhance RimWorld by introducing new behaviors and customization options for turret mechanics, specifically focusing on idle positioning. By providing more refined control over turret operations, players can optimize their defense strategies and manage their colonies more effectively. 

### Purpose
This mod aims to add depth to turret management by allowing players to set and manage idle positions for turrets. This adds a layer of strategic customization that can help mitigate the problem of turrets always facing the default position while idle.

## Key Features and Systems

- **Idle Position System**: Introduce a system where turrets can be assigned specific idle positions, enhancing tactical flexibility.
- **Gizmos Integration**: Provide user interface elements (gizmos) for interacting with turret settings within the game.
- **Turret Ticks Management**: Implement custom logic to manage turret behavior during game ticks, ensuring smooth and logical operations.

## Coding Patterns and Conventions

- **Static Helper Classes**: Utilized in classes like `Building_CMCTurretGun_Tick` and `Building_TurretGun_Tick` for modular and reusable code organization.
- **Public Method Accessibility**: Methods like those in `TurretIdlePositionGameComponent` are public to allow interaction across different parts of the mod, facilitating flexible and modular design.
- **C# Versioning**: Target .NET Framework 4.7.2 or newer, ensuring compatibility with the modding community’s standard environment.

## XML Integration

While XML configuration is not detailed in the provided summary, consider integration with RimWorld XML files for defining defaults or loading configurations. This approach may allow for dynamic adjustments based on game state or user preferences.

## Harmony Patching

- **Harmony**: An essential library for modifying game behavior without altering the base game code. Use Harmony to patch methods related to turret behavior to enable custom logic.
- **Target Methods**: Common targets include tick methods for turrets, UI gizmo functions, and initialization routines for the custom components.
- **Patch Examples**: No specific methods are mentioned, but look into adding prefixes or postfixes to existing methods within turret control flows to integrate new behaviors seamlessly.

## Suggestions for Copilot

- **Suggesting Code**: Encourage Copilot to propose code snippets that handle turret positioning logic, XML integration, and user interface enhancements.
- **Debugging Aids**: Implement logging within the Harmony patches and custom methods for improved debugging support.
- **Performance Considerations**: Highlight potential areas where code optimization could be suggested, particularly in the repeated logic of game tick operations.

By using these guidelines and instructions, developers can leverage GitHub Copilot effectively to enhance and maintain the RimWorld modding projects, ensuring high-quality contributions are made efficiently.
