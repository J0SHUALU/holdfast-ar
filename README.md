# Holdfast AR

A mobile AR survival shooter made in Unity 6 with AR Foundation, by **Joshua Chukwuebuka Moses**. Scan the floor, place the game on it, and survive until the timer runs out by shooting the enemies that spawn on the detected planes.

## Features

- **AR plane detection.** Only horizontal planes are detected, and the game is anchored to the plane you tap.
- **Custom plane tracker.** A textured mesh with **JOSHUA CHUKWUEBUKA MOSES** on it replaces Unity's default visualizer. It only shows while a plane is detected.
- **Tap to place.** Only one game can be placed. Extra taps do nothing, and plane detection stops after placing.
- **Player.** First person, with health, shooting, score, damage feedback (red flash and vibration) and a game over when health hits zero.
- **Object pooling.** Every bullet is created once at the start and reused. No Instantiate or Destroy during play, and bullets reset when they go back to the pool.
- **Melee enemy.** Moves toward you, only hits within 0.5 m, has an attack cooldown, and takes 3 shots.
- **Shooter enemy.** Moves toward you, stops at 1.4 m and shoots from up to 2.2 m, and takes 5 shots.
- **All enemies** spawn on the AR plane, have health, react when hit, add score when they die, and are cleared when the game ends.
- **Game states.** Start, Play and End, built with the State pattern.
- **UI.** Start menu (title, start, leaderboard, difficulty), in-game UI (health, score, time remaining) and end screen (final score, enemies defeated, time survived, restart, main menu).
- **Leaderboard.** Saves the latest 5 sessions and keeps them after the app closes.
- **Sound.** Player shoot, player death, enemy spawn, enemy shoot and melee attack, plus UI and ambient sounds.
- **Difficulty (bonus).** Easy, Normal and Hard change match length, health, spawn rate and enemy strength.

## Building the APK (Android)

1. Open the project in Unity 6 (6000.4) with Android Build Support installed.
2. Open `Assets/HoldfastAR/Scenes/HoldfastAR.unity`.
3. In **Project Settings > XR Plug-in Management > Android**, tick **Google ARCore**.
4. Go to **File > Build Profiles > Android**, switch platform, plug in an ARCore phone (Android 11 or newer) and press **Build And Run**.

## Assets

3D models from Quaternius on poly.pizza and UI sprites and font from Kenney on kenney.nl, all free (CC0). 

