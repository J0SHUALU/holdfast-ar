# Holdfast AR

A mobile **Augmented Reality survival shooter** built with **Unity 6** and **AR Foundation** (Android / ARCore and iOS / ARKit).
Scan your floor, tap to drop your *Holdfast* outpost onto a real surface, then survive waves of
enemies that spawn on the detected planes and hunt you down until the timer runs out.

Developer: **Joshua Chukwuebuka Moses**

---

## Gameplay

| | |
|---|---|
| **Perspective** | First-person: your phone is the player. Aim with the crosshair, hold **FIRE** to shoot. |
| **Goal** | Survive until the timer hits 0:00. Kill enemies for score. |
| **Lose** | Your health reaches 0. |
| **Brute (Melee)** | Red horned walker. Rushes you and hits only at close range (0.5 m) on a 1.1 s cooldown. 3 bullets to kill, 15 damage, 100 pts. |
| **Sentinel (Shooter)** | Blue hovering drone. Stops 1.4 m away and fires projectiles from up to 2.2 m. 5 bullets to kill, 8 damage per shot, 150 pts. |
| **Difficulty** | Easy / Normal / Hard change match length, player HP, spawn rate, max enemies, enemy speed & damage and score multiplier. |

Flow: **Main Menu → Scan & Place → Play → End Summary → Restart / Main Menu**

## Requirement checklist

- **AR plane detection & anchoring**: `ARPlaneManager` restricted to horizontal planes; the arena is attached to an `ARAnchor` on the tapped plane.
- **Custom plane tracker**: `NamedPlaneVisualizer` builds its own textured mesh from each plane boundary. The tiled texture shows **JOSHUA CHUKWUEBUKA MOSES** and only renders while a plane is tracked.
- **Tap to place, single instance**: later taps are ignored. Plane detection stops and the trackers hide once the arena is placed.
- **Player**: health, pooled shooting, score, red-flash + vibration damage feedback, game-over trigger.
- **Object pooling**: `ObjectPool<T>` pre-instantiates every bullet. No `Instantiate`/`Destroy` per shot; bullets are reset on reuse.
- **Two enemy types**: `Enemy` (abstract) → `MeleeEnemy`, `ShooterEnemy`. Different models, ranges, damage and bullets-to-kill.
- **Enemies**: spawn on detected planes, have health, flash/punch when hit, add score on death, and are wiped when the match ends.
- **Game states**: State pattern with `MainMenuState`, `PlacementState`, `PlayingState`, `GameOverState`.
- **UI**: start menu (title, start, leaderboard, difficulty), HUD (health, score, time), end summary (score, kills, time survived, restart, main menu).
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
    UI/         UIManager, UIPanel (abstract) + MainMenu/Leaderboard/Placement/Hud/GameOver panels
    GameManager.cs
  Resources/Audio/      12 original .wav sound effects
  Resources/Textures/   PlaneTrackerName.png (custom plane tracker texture)
  Art/                  AppIcon.png (app icon)
  Prefabs/ Materials/ Scenes/ Settings/
Tools/      Python scripts that build the audio, plane texture and app icon
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

Everything in this project is original:
- **Sound effects**: synthesised from scratch by `Tools/generate_audio.py` (sine/square/saw waves plus filtered noise).
- **Plane tracker texture**: drawn by `Tools/generate_plane_texture.py`.
- **App icon**: drawn by `Tools/generate_app_icon.py` (`Assets/HoldfastAR/Art/AppIcon.png`), set as the Default Icon for Android and iOS.
- **Enemy, base and blaster models**: assembled from Unity primitive meshes with custom materials.
- **UI**: built in code with uGUI and Unity's built-in font.
