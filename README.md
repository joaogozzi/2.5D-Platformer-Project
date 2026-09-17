# 2.5D Platformer Project

A 2.5D platformer prototype built in Unity, featuring `CharacterController`-based movement (jump, double jump, wall jump, dash), an inheritance-based enemy AI system, health/damage for both player and enemies, and the basic menu → gameplay → end screen flow.

## Table of Contents

- [Features](#features)
- [Script Overview](#script-overview)
- [Project Setup](#project-setup)
- [Controls](#controls)
- [Possible Next Steps](#possible-next-steps)

## Features

### Player
- Horizontal movement, jump and double jump.
- Wall slide and wall jump (slide down a wall and jump away from it).
- Dash with cooldown.
- Stomp: jumping on an enemy's head kills it instantly and gives the player a small upward bounce.
- Pushes `Rigidbody` objects (e.g. a dead enemy's corpse).
- Health with a brief invulnerability window after taking damage, plus knockback feedback (a small hop backward) when hit.

### Enemies
- Inheritance-based state machine: an abstract base class (`EnemyController`) handles health, damage, death, player detection and movement; each enemy type only implements its own patrol/chase/attack behavior.
- Two example enemies: melee (chases and attacks on contact) and ranged (keeps its distance and fires projectiles).
- Player detection via triggers (detection/attack zones), instead of searching for a `GameObject` by tag.
- Ledge detection during patrol (turns around before walking off a platform).
- On death, the enemy is replaced by a separate physical "corpse" (`Rigidbody` + `Collider`) that the player can push around like a box.

### Level
- Camera that follows the player smoothly, with look-ahead and optional scene bounds.
- Spike trap (contact damage with a cooldown).
- Collectible health pickup.
- Level-end trigger.

### Game Flow
- Main menu, pause, and end screen, with scene switching centralized in a single utility.

## Script Overview

| Script | Description |
|---|---|
| `PlayerCharacterController.cs` | Player movement, jump, wall jump, dash, stomp, push and damage feedback. |
| `PlayerHealth.cs` | Player health: damage, healing, temporary invulnerability, events (`OnHealthChanged`, `OnDeath`). |
| `PlayerStompDetector.cs` | Trigger on the player's feet; kills enemies when stomped on and triggers the bounce. |
| `IDamageable.cs` | Common interface (`TakeDamage`) implemented by both the player and enemies. |
| `EnemyController.cs` | Abstract base class for enemies: state, health, damage, death, detection, movement, edge detection. |
| `MeleeEnemyController.cs` | Melee enemy: patrols, chases and attacks on contact. |
| `RangedEnemyController.cs` | Ranged enemy: keeps its distance from the player and fires projectiles. |
| `EnemyDetectionZone.cs` | Trigger (detection/attack) used by enemies to find the player. |
| `Projectile.cs` | Projectile fired by ranged enemies; damages the player on hit. |
| `HealthPickup.cs` | Collectible item that heals the player on contact. |
| `SpikeTrap.cs` | Simple trap: deals contact damage with a cooldown. |
| `CameraFollow.cs` | Camera that follows the player with smoothing and look-ahead. |
| `SceneLoader.cs` | Static scene-switching, reload and quit utility, used by the menu/pause/end screen. |
| `MainMenuController.cs` | Main menu buttons (Play/Quit). |
| `PauseController.cs` | Game pause (`Time.timeScale`), with Resume/Restart/Menu/Quit. |
| `LevelEndTrigger.cs` | Trigger at the level's goal point; shows the end screen. |
| `EndScreenController.cs` | End screen buttons (Restart/Next Level/Main Menu). |

## Project Setup

### Tags
- `Player` — on the player's root GameObject.
- `Enemy` — on enemy GameObjects (used by `PlayerStompDetector`).

### Layers
- `Wall` — used by the player's wall jump (`wallCheckDistance`).
- `Ground` — used by the enemies' ledge detection (`EnemyController.IsEdgeAhead`).

Without these layers created with these exact names, the corresponding automatic fallbacks fail silently (`EnemyController` logs a warning to the Console when that happens).

### Animator Parameters

**Player:**
- `Side` (Int) — 0 idle, 1 right, 2 left
- `Speed` (Float), `IsGrounded` (Bool), `VerticalVelocity` (Float), `IsWallSliding` (Bool), `IsDashing` (Bool)
- `Jump`, `Attack`, `Hurt` (Triggers)

**Enemies:**
- `Speed` (Float), `IsGrounded` (Bool), `IsChasing` (Bool)
- `Attack`, `Hurt`, `Death` (Triggers)

### Prefab Components

- **Player**: `CharacterController`, `Animator`, `PlayerCharacterController`, `PlayerHealth`, plus a child object with a trigger `Collider` + `PlayerStompDetector`.
- **Enemy (alive)**: `CharacterController`, `Animator`, `MeleeEnemyController` or `RangedEnemyController`, plus children with trigger `Collider`s + `EnemyDetectionZone` (one for detection, one for attack). References a separate `corpsePrefab`.
- **Enemy corpse** (separate prefab): the enemy's visual + a regular `Collider` + a non-kinematic `Rigidbody` — no AI scripts at all.
- **Projectile** (used by the ranged enemy): `Collider`, `Rigidbody`, `Projectile`.

### Scenes
All scenes in use (menu, levels) need to be added to `File > Build Settings > Scenes In Build` for `SceneManager.LoadScene` to work by name.

## Controls

| Action | Input |
|---|---|
| Move | Configured in `InputManager` (`OnPlayerMove`) |
| Jump / Double Jump / Wall Jump | `OnJump` |
| Dash | `OnDash` |
| Pause | `Escape` (configurable in `PauseController`) |

## Possible Next Steps

- Player melee attack (currently only the `Attack` state exists; the actual hitbox/damage is still missing).
- A full `Hurt` state for the player (right now knockback only locks horizontal movement).
- Sound and particle feedback (damage, death, pickup, dash).
- Save/checkpoint system between levels.
