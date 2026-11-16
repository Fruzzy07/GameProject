# Задачи Азамата

Я(Ернур) сделал **Mechanics (10)** и **Inputs (4)**: башни, врагов, волны, беркута, Кош и управление.
Твоя часть - **Progress, Visuals, UI, Settings, Content** (3 + 3 + 3 + 2 + 3 = 14 баллов).

Сейчас в игре временный текстовый HUD (`DebugHud`): он нужен только чтобы проверять механику.
Твоя задача - превратить это в настоящую игру с меню, сохранениями и красивым интерфейсом.

## Как начать

```bash
git clone https://github.com/Fruzzy07/GameProject.git
cd GameProject
git checkout develop
git checkout -b feature/azamat-ui
```

Открой проект в Unity 6 (6000.3.23f1), сцену `Assets/Scenes/Level_01.unity`, нажми Play.
Управление описано в [README](../README.md).

---

## 1. UI - 3 балла

Критерий: *UI contains necessary information and adaptive to different screens. Game includes main menu.*

- [ ] **Главное меню** - новая сцена `MainMenu`: название игры, кнопки Play, Settings, Quit.
- [ ] **HUD уровня** вместо `DebugHud`:
  - жизни (иконка `icon_shanyrak`), золото (иконка `icon_tenge`), номер волны;
  - кнопки: вызвать волну (`WaveSpawner.CallEarly()`), скорость (`GameSpeed.ToggleFast()`), пауза, беркут (`PlayerInputController.BeginBerkut()`) с полоской перезарядки (`BerkutAbility.Charge`).
- [ ] **Меню паузы**: Resume, Restart, Main menu.
- [ ] **Экран победы и поражения** со звёздами.
- [ ] **Canvas Scaler** → Scale With Screen Size, 1920×1080: интерфейс должен нормально выглядеть на любом разрешении.
- [ ] Для фона панелей и кнопок есть готовые спрайты `Assets/Art/Sprites/UI/ui_panel.png` и `ui_button.png`.

## 2. Settings - 2 балла

Критерий: *Game settings can be changed in menu. Changes are saved between sessions.*

- [ ] Окно настроек в главном меню: громкость музыки, громкость звуков, полный экран.
- [ ] Настройки сохраняются (`PlayerPrefs` или JSON-файл) и применяются при следующем запуске.
- [ ] Громкость лучше делать через **AudioMixer** (тема недели 8): группы Music и SFX, параметры `MusicVolume` и `SfxVolume`.

## 3. Progress - 3 балла

Критерий: *Game has some kind of progression... Game saves some data between sessions.*

- [ ] **Звёзды за уровень**: осталось 18+ жизней - 3 звезды, 10+ - 2, иначе 1.
- [ ] **Сохранение**: лучший результат по звёздам для каждого уровня.
- [ ] **Открытие уровней**: уровень 2 открывается после победы на уровне 1.

## 4. Content - 3 балла

Критерий: *Player can enjoy different types of content. More than one level, type of enemy, upgrades, etc.*

Уже готово: 4 башни с 3 уровнями улучшений и 5 типов врагов (`Assets/Data`).

- [ ] **Уровень 2** (и дальше, если успеешь): новая сцена с другой дорогой (точки `EnemyPath`), другими местами под башни и новыми волнами.
  Новый уровень = новый `LevelData` ассет (`Create → Steppe Bastion → Level`).
- [ ] **Экран выбора уровня** (или карта мира): уровни со звёздами, закрытые - серые.

## 5. Visuals - 3 балла

Критерий: *Game have a consistent visual style, no graphics artifacts or glitches.*

- [ ] **Анимации через Animator** (неделя 6): враги покачиваются при ходьбе, башня «сжимается» при выстреле, беркут машет крыльями.
  Префабы лежат в `Assets/Prefabs`.
- [ ] **Звук и музыка** (неделя 8): выстрелы башен, смерть врага, постройка, победа, поражение, фоновая музыка.
  Места, куда добавить звуки: `Tower.Attack`, `Enemy.Die`, `Enemy.ReachBase`, `BuildMenu` (постройка, улучшение, продажа), `BerkutAbility.Strike`, `WaveSpawner.StartNextWave`.
- [ ] Единый стиль: интерфейс в тех же цветах, что и графика (красный, золотой, синий, казахский орнамент).

---

## Что можно использовать из моего кода

| Что нужно | Где взять |
|---|---|
| Жизни и золото поменялись | `LevelManager.Instance.LivesChanged`, `GoldChanged` (события) |
| Победа или поражение | `LevelManager.Instance.StateChanged` → `GameState.Won` / `Lost` |
| Сколько жизней осталось (для звёзд) | `LevelManager.Instance.Lives`, `Level.startLives` |
| Номер волны, таймер | `WaveSpawner.CurrentWave`, `TotalWaves`, `Countdown`, `CanCallEarly`, `EarlyCallBonus` |
| Пауза и скорость | `GameSpeed.SetPaused(bool)`, `GameSpeed.ToggleFast()`, событие `GameSpeed.Changed` |
| Беркут | `BerkutAbility.Ready`, `Charge` (0..1), `PlayerInputController.BeginBerkut()` |
| Подсказка «что сейчас делает клик» | `PlayerInputController.ModeChanged` |
| Перезапуск уровня | `SceneLoader.RestartLevel()` |

Когда сделаешь свой HUD, удали `DebugHud` со сцены.
