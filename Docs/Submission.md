# Holdfast AR: Submission

**Student:** Joshua Chukwuebuka Moses
**Project:** Holdfast AR, a mobile AR survival shooter (Unity 6 + AR Foundation)

| # | Deliverable | Link |
|---|---|---|
| 1 | GitHub repository (includes `.gitignore`; `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `Build/` are excluded) | https://github.com/J0SHUALU/holdfast-ar |
| 2 | APK build: **Android** (ARCore, ARM64, Android 7.0 / API 24+) | _paste APK link here_ |
| 3 | Technical documentation (PDF) | https://github.com/J0SHUALU/holdfast-ar/blob/main/Docs/TechnicalDocumentation.pdf |
| 4 | Video demonstration (on-device, starting from the Unity splash screen) | _paste video link here_ |

## Summary

- Horizontal AR plane detection with a custom plane tracker texture showing **JOSHUA CHUKWUEBUKA MOSES**.
- Tap-to-place a single anchored arena; plane detection stops after placement.
- First-person shooter: health, score, pooled shooting, damage feedback, game-over trigger.
- Two enemy types: **Brute** (melee, 3 hits) and **Sentinel** (shooter, 5 hits), spawning on detected planes.
- Object pooling for every projectile (no Instantiate/Destroy during gameplay).
- Game states: Main Menu → Placement → Playing → Game Over (State pattern).
- Local leaderboard with the latest 5 sessions, persisted between launches.
- Full sound design: shoot, death, spawn, enemy shoot, melee attack, plus hit, UI and music.
- Easy / Normal / Hard difficulty.
- Patterns: Object Pool, State, Singleton, Factory, Observer.
