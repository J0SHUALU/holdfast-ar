# Holdfast AR

A mobile **Augmented Reality survival shooter** built with **Unity 6** and **AR Foundation** (Android / ARCore and iOS / ARKit).
Scan your floor, tap to drop your *Holdfast* outpost onto a real surface, then survive waves of
enemies that spawn on the detected planes and hunt you down until the timer runs out.

**Story:** you are the last colonist on the frontier world Kepler-9. Your landing dome has come down on the only flat ground you can find, and the planet's natives are closing in. Hold the dome with your ray gun until the evac ship arrives.

Developer: **Joshua Chukwuebuka Moses**

---

## Gameplay

| | |
|---|---|
| **Perspective** | First-person, portrait: your phone is the player. Aim with the crosshair, hold **FIRE** to shoot. |
| **Goal** | Survive until the timer hits 0:00. Kill enemies for score. |
| **Lose** | Your health reaches 0. |
| **Brute (Melee)** | Green alien grunt. Rushes you and hits only at close range (0.5 m) on a 1.1 s cooldown. 3 bullets to kill, 15 damage, 100 pts. |
| **Sentinel (Shooter)** | Yellow bug mech. Stops 1.4 m away and fires projectiles from up to 2.2 m. 5 bullets to kill, 8 damage per shot, 150 pts. |
| **Difficulty** | Easy / Normal / Hard change match length, player HP, spawn rate, max enemies, enemy speed & damage and score multiplier. |

Flow: **Main Menu → Scan & Place → Play → End Summary → Restart / Main Menu**

## Requirement checklist

- **AR plane detection & anchoring**: `ARPlaneManager` restricted to horizontal planes; the arena is attached to an `ARAnchor` on the tapped plane.
- **Custom plane tracker**: `NamedPlaneVisualizer` builds its own textured mesh from each plane boundary. The tiled texture shows **JOSHUA CHUKWUEBUKA MOSES** and only renders while a plane is tracked.
- **Tap to place, single instance**: later taps are ignored. Plane detection stops and the trackers hide once the arena is placed.
- **Player**: health, pooled shooting, score, red-flash + vibration damage feedback, game-over trigger.
- **Object pooling**: `ObjectPool<T>` pre-instantiates every bullet. No `Instantiate`/`Destroy` per shot; bullets are reset on reuse.
- **Two enemy types**: `Enemy` (abstract) → `MeleeEnemy`, `ShooterEnemy`. Different models, ranges, damage and bullets-to-kill.
- **Enemies**: spawn on detected planes (a marker shows where), have health, play hit reactions, add score on death, and are wiped when the match ends.
- **Game states**: State pattern with `MainMenuState`, `PlacementState`, `PlayingState`, `GameOverState`.
- **UI**: portrait layout. Start menu (title, start, leaderboard, difficulty), HUD (health, score, time, spawn markers), end summary (score, kills, time survived, restart, main menu).
- **Leaderboard**: latest 5 sessions saved as JSON in `PlayerPrefs`, so they persist between launches.
- **Sound**: player shoot, player death, enemy spawn, enemy shoot, melee attack, plus hit, death, UI, placement, victory and ambient music.

## Project structure

```
Assets/HoldfastAR/
  Scripts/
    Core/       Singleton<T>, GameEvents (observer), IDamageable, Team, InputHelper
    States/     GameState, GameStateMachine, MainMenu/Placement/Playing/GameOver states
    AR/         ARPlacementController, NamedPlaneVisualizer, PlaneSpawnArea
    Player/     PlayerHealth, PlayerWeapon
    Enemies/    Enemy (abstract), MeleeEnemy, ShooterEnemy, EnemyFactory, EnemySpawner
    Pooling/    IPoolable, ObjectPool<T>
    Combat/     Projectile, ProjectilePool
    Audio/      AudioManager, SoundId
    Data/       DifficultySettings, GameSession, Leaderboard, SessionRecord
    UI/         UIManager, UIPanel (abstract) + MainMenu/Leaderboard/Placement/Hud/GameOver panels, SpawnMarkers
    GameManager.cs
  Resources/Audio/      12 original .wav sound effects
  Resources/Textures/   PlaneTrackerName.png (custom plane tracker texture)
  Resources/UI, Fonts/  UI sprites, app icon and font
  Art/                  Models, textures and Animator Controllers
  Prefabs/ Materials/ Scenes/ Settings/
Tools/      Python scripts that build the audio and the plane texture
Docs/       Technical documentation (PDF + HTML source) and the submission document
```

## Building (Android)

1. Open the project in **Unity 6** (6000.4) with the Android Build Support module installed.
2. Open `Assets/HoldfastAR/Scenes/HoldfastAR.unity`.
3. In **Project Settings → XR Plug-in Management → Android**, make sure **Google ARCore** is ticked. Run **Project Validation → Fix All** if anything is flagged.
4. **File → Build Profiles → Android → Switch Platform**, plug in an ARCore-supported phone, then **Build And Run**.

`Assets/Plugins/Android/gradleTemplate.properties` sets `android.uniquePackageNames=false`. Keep it: without it Gradle stops at the manifest merge because two ARCore libraries share the `com.google.ar.core` namespace.

## Building (iOS)

1. Install the iOS Build Support module for Unity 6 (6000.4) and Xcode on a Mac.
2. In **Project Settings → XR Plug-in Management → iOS**, make sure **Apple ARKit** is ticked.
3. **File → Build Profiles → iOS → Switch Platform**, then **Build** into a folder (for example `Builds/iOS`).
4. Open `Unity-iPhone.xcodeproj` in Xcode, select the **Unity-iPhone** target, and under **Signing & Capabilities** tick *Automatically manage signing* and choose your Apple ID team.
5. Plug in an ARKit-capable iPhone (iOS 15+), select it as the run destination and press **Run**. The first time, trust the developer profile on the phone under *Settings → General → VPN & Device Management*.

Bundle identifier: `com.joshuamoses.holdfastar` on both platforms.

## Assets & credits

| Asset | Used for | Source | Licence |
|---|---|---|---|
| Ultimate Space Kit by Quaternius | Brute (Enemy Large), Sentinel (Mech), dome, rocks, plants, solar panel | poly.pizza | CC0 |
| Sci-Fi Gun Pack by Quaternius | Player ray gun | poly.pizza | CC0 |
| UI Pack (Space Expansion) by Kenney | Panels, buttons, bars, crosshair, spawn markers | kenney.nl | CC0 |
| Kenney Future font | UI text and plane tracker label | kenney.nl | CC0 |

Made for this project:
- **Sound effects**: synthesised by `Tools/generate_audio.py`.
- **Plane tracker texture**: drawn by `Tools/generate_plane_texture.py`.
- **App icon**: a render of the Brute model inside a red reticle (`Assets/HoldfastAR/Resources/UI/AppIcon.png`), set as the Default Icon for Android and iOS.
- **Animator Controllers, materials and UI layout**: built in Unity.
