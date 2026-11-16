# Steppe Bastion

2D tower defense set in the Kazakh Khanate. Creatures from Kazakh legends attack the nomad aul,
and the player defends Ak Orda, the khan's white yurt.

Course project: Unity Game Engine for Software Engineers, KBTU, Fall 2026.
Team: Yernur Shapkat, Azamat Taubayev.

- Unity 6 (6000.3.23f1), URP 2D, New Input System
- Main scene: `Assets/Scenes/Level_01.unity`

## Who does what

| Criteria | Points | Owner |
|---|---|---|
| Inputs | 4 | Yernur |
| Mechanics | 10 | Yernur |
| Progress | 3 | Azamat |
| Visuals | 3 | Azamat |
| UI | 3 | Azamat |
| Settings | 2 | Azamat |
| Content | 3 | Azamat |

Azamat's task list: [docs/TASKS_AZAMAT.md](docs/TASKS_AZAMAT.md).

## Controls

| Action | Key |
|---|---|
| Build, upgrade, sell, move a tower (Kosh) | Left click on a build spot or tower |
| Berkut strike | B or right mouse button, then click |
| Call the next wave early (bonus gold) | Space |
| Speed x1 / x2 | F |
| Pause, cancel | Esc |
| Restart after victory or defeat | R |

## Project structure

```
Assets/
  Art/            sprites and level background
  Data/           ScriptableObjects: towers, enemies, levels (waves)
  Prefabs/        Enemy, Tower, Projectile, effects, berkut
  Scenes/         Level_01 (game), Yernur / Azamat (test scenes)
  Scripts/Runtime/
    Controls/     PlayerInputController: mouse and keyboard through the Input System
    Core/         LevelManager (gold, lives, win/lose), WaveSpawner, EnemyPath, GameSpeed, SceneLoader
    Data/         TowerData, EnemyData, LevelData
    Enemies/      Enemy, HealthBar
    Heroes/       BerkutAbility
    Towers/       Tower, BuildSpot, Projectile, HitEffect, LightningEffect
    UI/           BuildMenu (radial menu), DebugHud (temporary text HUD), UIFactory
```

## Git workflow

- Work on `develop`, each of us in our own feature branch: `feature/<name>`.
- Pull before you start: `git pull origin develop`.
- Don't edit the same scene at the same time: scene merges in Unity are painful.
  Build your UI in your own scene or prefab, then add it to `Level_01` in one commit.
