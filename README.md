# Holdfast AR

Holdfast AR is a survival shooter for phones, built in Unity 6 with AR Foundation. It runs on Android (ARCore) and iPhone (ARKit).

You play the last colonist on a planet called Kepler-9. Your landing dome has come down on the only flat ground around, which is your real floor, and the locals aren't friendly. Scan the floor, drop the dome, and hold off three kinds of alien attackers with your ray gun until the evac ship arrives.

Made by **Joshua Chukwuebuka Moses**.

## How it plays

You hold the phone in portrait and the phone is you. Aim with the red crosshair and hold **FIRE** to shoot. Last until the timer hits 0:00 and the dome holds. Run out of health and you're overrun.

| Enemy | What it does | Shots to kill | Damage | Score |
|---|---|---|---|---|
| **Brute** (melee) | Green alien. Walks straight at you and punches once it's within half a metre, every 1.1 s. | 3 | 15 | 100 |
| **Sentinel** (shooter) | Yellow bug mech. Stops 1.4 m away and shoots from up to 2.2 m. | 5 | 8 per shot | 150 |
| **Glider** (diver) | Purple flyer. Cruises above the floor, then dives at your phone and bursts on impact. | 1 | 20 | 75 |

Easy, Normal and Hard change the match length, your health, how fast enemies spawn, how many can be out at once, how fast and hard they hit, and your score multiplier.

The flow goes Main Menu, then scan and place, then play, then the end screen, where you can restart or head back to the menu.

## What's in it

- **Plane detection and anchoring.** Only horizontal planes are detected. Tapping one attaches an anchor and the dome sits on it.
- **Custom plane tracker.** `NamedPlaneVisualizer` builds its own mesh from each plane's outline and tiles a texture with **JOSHUA CHUKWUEBUKA MOSES** across it. It only shows while a plane is tracked.
- **One dome only.** Extra taps do nothing. Once the dome lands, plane detection stops and the trackers hide.
- **Player.** Health, shooting, score, a red flash and vibration when you get hit, and a game over when health runs out.
- **Object pooling.** Every bullet is made once at the start by `ObjectPool<T>` and reused. Nothing is created or destroyed per shot, and bullets reset when they come back.
- **Three enemy types.** `MeleeEnemy`, `ShooterEnemy` and `DiverEnemy` all inherit from the abstract `Enemy`. They spawn on real planes (a small HUD marker shows where), take hits with a reaction, add score when they die, and get cleared when the match ends.
- **Game states.** `MainMenuState`, `PlacementState`, `PlayingState` and `GameOverState`, run by a small state machine.
- **UI.** Built for portrait. Start menu with title, start, leaderboard, settings and difficulty. A settings screen for sound, music, vibration and spawn markers. A HUD with health, score and time. An end screen with score, kills, time survived, restart and main menu.
- **Leaderboard.** The latest 5 matches are saved as JSON in `PlayerPrefs`, so they're still there after you close the app.
- **Sound.** Player shoot, player death, enemy spawn, enemy shoot and the melee hit, plus hit, death, UI, placement, victory and ambient music.

## Project layout

```
Assets/HoldfastAR/
  Scripts/
    Core/       Singleton<T>, GameEvents, IDamageable, Team, InputHelper
    States/     GameState, GameStateMachine and the four states
    AR/         ARPlacementController, NamedPlaneVisualizer, PlaneSpawnArea
    Player/     PlayerHealth, PlayerWeapon
    Enemies/    Enemy, MeleeEnemy, ShooterEnemy, DiverEnemy, EnemyFactory, EnemySpawner
    Pooling/    IPoolable, ObjectPool<T>
    Combat/     Projectile, ProjectilePool
    Audio/      AudioManager, SoundId
    Data/       DifficultySettings, GameSettings, GameSession, Leaderboard, SessionRecord
    UI/         UIManager, UIPanel and the six screens, SpawnMarkers
    GameManager.cs
  Editor/               IOSBuildSettings (Xcode linker settings for ARKit)
  Resources/Audio/      the 12 sound effects
  Resources/Textures/   PlaneTrackerName.png, the plane tracker texture
  Resources/UI, Fonts/  UI sprites, app icon and font
  Art/                  models, textures and animator controllers
  Prefabs/ Materials/ Scenes/ Settings/
Tools/      Python scripts for the sounds and the plane texture
Docs/       submission document (Word) and technical documentation (PDF)
```

The scene looks fairly empty in the editor. That's expected. The menus and HUD are built from code when the game starts, and the dome, enemies and bullets are prefabs that get spawned during play. Press Play to see everything.

## Building for Android

1. Open the project in Unity 6 (6000.4) with Android Build Support installed.
2. Open `Assets/HoldfastAR/Scenes/HoldfastAR.unity`.
3. In **Project Settings > XR Plug-in Management > Android**, tick **Google ARCore**. If Project Validation flags anything, hit Fix All.
4. Go to **File > Build Profiles > Android**, switch platform, plug in an ARCore phone and press **Build And Run**.

Leave `Assets/Plugins/Android/gradleTemplate.properties` in place. It sets `android.uniquePackageNames=false`, and without it Gradle fails at the manifest merge because two ARCore libraries share the `com.google.ar.core` namespace.

## Building for iPhone

1. Install iOS Build Support for Unity 6 (6000.4) and Xcode on a Mac.
2. In **Project Settings > XR Plug-in Management > iOS**, tick **Apple ARKit**.
3. Go to **File > Build Profiles > iOS**, switch platform first, then build into a folder like `Builds/iOS`. The switch matters: ARKit only adds its camera background shader when iOS is the active platform, and without it the camera shows black. `Editor/IOSBuildSettings.cs` adds the Swift library paths that Xcode 26 needs to link ARKit.
4. Open `Unity-iPhone.xcodeproj`, pick the **Unity-iPhone** target, and under **Signing & Capabilities** turn on automatic signing with your Apple ID team.
5. Plug in an ARKit iPhone (iOS 15 or newer), choose it as the destination and press **Run**. The first time, trust your developer profile on the phone under *Settings > General > VPN & Device Management*.

The bundle ID is `com.joshuamoses.holdfastar` on both platforms.

## Assets and credits

| Asset | Used for | Source | Licence |
|---|---|---|---|
| Ultimate Space Kit by Quaternius | Brute, Sentinel, Glider, dome, rocks, plants, solar panel | poly.pizza | CC0 |
| Sci-Fi Gun Pack by Quaternius | The ray gun | poly.pizza | CC0 |
| UI Pack (Space Expansion) by Kenney | Panels, buttons, bars, crosshair, spawn markers | kenney.nl | CC0 |
| Kenney Future font | UI text and the plane tracker label | kenney.nl | CC0 |

I made the rest myself:

- The 12 sound effects, built from simple waveforms and noise by `Tools/make_audio.py`.
- The plane tracker texture, drawn by `Tools/make_plane_texture.py`.
- The app icon, a render of the Brute inside a red target, used for both Android and iOS.
- The animator controllers, materials and the UI layout.
