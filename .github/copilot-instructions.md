# GitHub Copilot Instructions for Machine Gun Defense (Continued)

## Mod Overview and Purpose
Machine Gun Defense (Continued) is a RimWorld mod that adds various anti-air machine gun turrets and flak guns. The mod enhances the game by providing a range of new turrets, each requiring specific research and resources to unlock and build. It supports multiple languages, including English, Chinese, Korean, and Japanese.

## Key Features and Systems
### Turrets
- **7.62mm Gun Turrets**: Maxim, Quad Maxim
- **12.7mm Gun Turrets**: M2 Browning, Twin M2, M45 Quadmount
- **20mm Gun Turrets**: Oerlikon 20mm, Twin Oerlikon with Shield, Quad Oerlikon 20mm
- **40mm Gun Turrets**: Bofors 40mm Auto Cannon, Mk.1 Dual Bofors, Mk.2 Quad Bofors
- **Automated Turrets**: C-RAM (CIWS) turret

### Research Requirements
- **7.62mm machine gun turret**: Requires Blowback operation research
- **12.7mm machine gun turret**: Requires Gas operation research
- **20mm autocannon and 40mm flak gun**: Requires Autocannon research in the AAA tab
- **CIWS automated turret**: Requires Gatling research in the AAA tab

### Bulk Recipes for Ammo
Added bulk recipes for ammunition to make it easier to produce.

## Coding Patterns and Conventions
### C# Code Structure
The mod follows a structured C# code pattern with well-defined classes and methods. Key components include:

- **Comps**: Custom components for turrets, such as `CompTurretTopSize`, `CompProjectileSmoke`, and `CompReproductiveProjectile`.
- **Harmony Patches**: Used to modify vanilla RimWorld behavior without altering the core game files.

### XML Integration
XML files are used to define game assets such as recipes, sound definitions, and other game objects. Key XML files include:

- `RecipeDefs`: Defines recipes for building and crafting turrets.
- `SoundDefs`: Defines sound effects for turrets.
- `ThingDefs`: Defines new turrets and their properties.

## Harmony Patching
Harmony is a popular library for modding that allows developers to patch vanilla RimWorld behavior without altering the core game files. This mod uses Harmony to:

- Modify building behavior.
- Add custom sound effects.
- Implement new turret functionalities.

## Suggestions for Copilot
### General
- **Code Snippets**: Request code snippets for specific functionalities, such as creating new turrets or modifying existing ones.
- **XML Templates**: Request XML templates for defining new game assets like recipes or sound effects.

### C# Specific
- **Custom Components**: Request examples of custom components like `CompTurretTopSize` or `CompProjectileSmoke`.
- **Harmony Patches**: Request examples of Harmony patches to modify vanilla behavior.

### XML Specific
- **RecipeDefs**: Request examples of XML definitions for recipes, such as those found in `Recipes_12.7mm.xml`.
- **SoundDefs**: Request examples of XML definitions for sound effects, such as those found in `AAA_50Cal.xml`.

By following these guidelines and suggestions, you can effectively leverage GitHub Copilot to enhance your development workflow for the Machine Gun Defense (Continued) mod.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

