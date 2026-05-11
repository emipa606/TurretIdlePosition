# GitHub Copilot Instructions for Turret Idle Position Mod

## Mod Overview and Purpose

**Mod Name:** Turret Idle Position  
**Mod Description:** This RimWorld mod allows players to set a default idle position for turrets, pointing them towards a specified degree when not actively engaged. Turrets can also be constrained to an optional allowed arc while idle. This feature is primarily cosmetic and does not influence the turret's firing arc. The mod also applies to manned turrets, resetting them to a defined idle position when unmanned. The intention behind this mod is to satisfy the cosmetic desire for turrets to face a particular direction when inactive, enhancing visual appeal and thematic consistency in a player's base setup.

## Key Features and Systems

- **Idle Position Degrees:** Set a default rotation degree for the turret's idle stance.
- **Movement Arc:** Optionally define an arc within which the turret can adjust its position when idle.
- **Cosmetic Focus:** The mod does not alter combat mechanics, ensuring the firing arc remains unaffected.
- **Functionality on Manned Turrets:** Includes functionality for both automated and manned turrets, transitioning them back to their idle settings when unmanned.

## Coding Patterns and Conventions

- **Class Utilization:** The mod utilizes a mix of static and instance classes to organize functionality.
- **Public Methods:** Methods like `AddTurretIdlePosition`, `RemoveTurretIdlePosition`, and `TryGetTurretIdlePosition` are exposed to manage turret configurations effectively.
- **Static Classes:** Used for patching and utility methods to maintain method accessibility and control over function execution.

## XML Integration

- XML configuration is not explicitly mentioned, implying most data settings are likely hardcoded or managed through C#.

## Harmony Patching

- **Use of Harmony:** The mod employs Harmony patches to integrate with RimWorld's core mechanics without altering the base game's source code.
- **Patch Targets:** Likely targets specific methods related to turret behaviors, such as their tick updates or GUI elements for setting positions.

## Suggestions for Copilot

- **XML Documentation Comments:** Encourage Copilot to include XML documentation comments for public methods and classes to improve code readability and auto-generated documentation.
- **Refactoring Patterns:** Suggest Copilot offer refactoring options for repetitive code structures within the patches, enhancing maintainability.
- **Ensuring Compatibility:** When suggesting code, Copilot should consider mod compatibility with different RimWorld versions by abstracting version-dependent logic.
- **Enhanced Guard Clauses:** Encourage Copilot to integrate guard clauses in methods to handle edge cases, such as null references when managing turret positions.
- **Code Readability Enhancements:** Suggests formatting improvements and comments for complex logic segments, aiding in code comprehension for future developers.

## Files Summary

- **Assembly Info and Versioning:** Various assembly files for .NET Framework version management.
- **Core Files:**
  - `Building_CMCTurretGun_Tick.cs`: Handles tick-rate behavior for CMC turrets.
  - `Building_Turret_GetGizmos.cs`: Manages interface elements for customizing turret settings.
  - `Building_TurretGun_Tick.cs`: Manages idle and active tick behavior for turret guns.
  - `Dialog_TurretIdlePosition.cs`: Implements a user interface to set and modify turret idle positions.
  - `ThingWithComps_PreSwapMap` and `ThingWithComps_PostSwapMap`: Manages turret behaviors related to map transitions.
  - `TurretIdlePosition.cs`: Centralized static class for managing idle position logic.
  - `TurretIdlePositionGameComponent.cs`: Handles game-specific component logic for tracking and applying idle positions.
  - `TurretTop_TurretTopTick.cs`: Controls the tick behavior specifically for the turret top, influencing rotation logic.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
