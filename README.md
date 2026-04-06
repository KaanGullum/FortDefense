# Fort Defense Prototype

Stylized 3D Unity tower defense / base defense prototype built around a fixed road with square build slots next to the lane. The project targets iPhone landscape first and keeps the first vertical slice focused on readable placement, waves, combat, and a light production loop.

## What Is Included

- `1` main menu scene
- `1` playable battle scene
- `4` enemy waves
- `3` defense buildings: Gun Tower, Cannon Tower, Mortar Tower
- `4` economy buildings: Mine, Generator, Smelter, Ammo Factory
- `3` enemy types: Runner, Brute, Armored
- fixed road/path with visible square build slots
- special ore, power, and industry build slots
- base/core health, win, and lose states
- mobile-friendly tap placement UI
- selected-building panel with upgrade and sell actions

## Project Structure

- `Assets/Scripts/Core`
  Scene bootstrappers and scene loading.
- `Assets/Scripts/Gameplay`
  Map generation, tile selection, placement, resources, and core health.
- `Assets/Scripts/Buildings`
  Shared building behavior, towers, economy cycles, and projectiles.
- `Assets/Scripts/Enemies`
  Enemy creation, path following, health, and damage handling.
- `Assets/Scripts/Waves`
  Wave countdowns and spawning.
- `Assets/Scripts/UI`
  Main menu and battle HUD/build menu.
- `Assets/Scripts/Data`
  ScriptableObject definitions and tunable gameplay data.
- `Assets/Scripts/Editor`
  Unity editor setup that generates scenes and default ScriptableObject assets.

## How To Run

1. Open the repository root in Unity `2022.3 LTS` or newer.
2. Let scripts compile. On first open, the editor setup script creates:
   - `Assets/Scenes/MainMenu.unity`
   - `Assets/Scenes/Battle.unity`
   - gameplay ScriptableObjects under `Assets/Resources/GameConfigs/`
3. If you want to regenerate everything manually, run `Tools/Fort Defense/Rebuild Prototype Assets & Scenes`.
4. Open `Assets/Scenes/MainMenu.unity` and press Play.

## How Configuration Works

- building stats and costs live in `Assets/Resources/GameConfigs/Buildings`
- enemy stats live in `Assets/Resources/GameConfigs/Enemies`
- wave composition lives in `Assets/Resources/GameConfigs/Waves`
- starting resources and core health live in `Assets/Resources/GameConfigs/GameBalanceConfig.asset`
- prototype asset versioning is tracked in `GameBalanceConfig`, so opening the project after script changes can regenerate defaults automatically

The runtime loads these assets from `Resources`, so you can rebalance the prototype in the inspector without editing scene wiring.

## Current Gameplay Loop

- Tap a highlighted build slot near the road.
- Choose a structure from the build panel.
- Economy buildings generate or convert ore, energy, alloy, and ammo over time.
- Special ore/power/industry slots boost the right economy buildings.
- Defense buildings automatically target enemies, but now consume live power and/or ammo when firing.
- Select an existing structure to inspect status, upgrade it, or sell it back for a refund.
- If enemies reach the core, the core loses health.
- Survive all four waves to win.

## Complete In This Vertical Slice

- handcrafted road-and-slots layout
- tile selection, highlighting, and one-building-per-slot placement
- multiple economy and defense building types
- explicit supply-chain dependency between production and stronger defenses
- selected-building inspector with status, upgrade, and sell flow
- resource-node style tile bonuses for map readability and strategic placement
- projectile combat including AoE mortar shots
- fixed-path enemy movement
- wave countdowns, progression, and victory/defeat overlays
- iPhone landscape-oriented HUD and build menu

## Placeholder / Next Steps

- visuals use generated primitive meshes and simple colors
- no audio yet
- logistics is still lightweight: there are no conveyors, transport units, or storage caps yet
- no custom VFX, animation, or save/load
- no storage/depot system yet
- no multi-entry path layout yet
- no siege unit or tesla tower yet

## Notes

- The battle map and object layout are generated at runtime for clarity and easy extension.
- Scenes are intentionally minimal boot scenes; the editor script wires them and sets build settings automatically.
- The project is designed to be a clean prototype foundation rather than final content production.
