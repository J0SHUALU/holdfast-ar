"""
Procedural sound generator for Holdfast AR.

Every sound effect in the game is synthesised here from sine/square waves and
filtered noise, so all audio is original and license-free.
Run:  python Tools/generate_audio.py
Output: Assets/HoldfastAR/Resources/Audio/<SoundId>.wav  (16-bit mono, 44.1 kHz)
"""
import os
import wave

import numpy as np

SR = 44100
OUT = os.path.join(os.path.dirname(__file__), "..", "Assets", "HoldfastAR", "Resources", "Audio")
rng = np.random.default_rng(7)


def t_axis(seconds):
    return np.linspace(0.0, seconds, int(SR * seconds), endpoint=False)


def sweep(f0, f1, seconds, shape="sine"):
    """Frequency sweep with exponential glide from f0 to f1."""
    t = t_axis(seconds)
    freq = f0 * (f1 / f0) ** (t / seconds)
    phase = 2 * np.pi * np.cumsum(freq) / SR
    if shape == "square":
        return np.sign(np.sin(phase))
    if shape == "saw":
        return 2 * ((phase / (2 * np.pi)) % 1.0) - 1
    return np.sin(phase)


def noise(seconds):
    return rng.uniform(-1, 1, int(SR * seconds))


def lowpass(signal, amount):
    """Simple one-pole low-pass. amount in (0,1): lower = darker."""
    out = np.zeros_like(signal)
    acc = 0.0
    for i, x in enumerate(signal):
        acc += amount * (x - acc)
        out[i] = acc
    return out


def env(seconds, attack=0.005, decay=8.0):
    t = t_axis(seconds)
    a = np.clip(t / max(attack, 1e-6), 0, 1)
    return a * np.exp(-decay * t)


def fade_edges(signal, ms=4):
    n = int(SR * ms / 1000)
    signal[:n] *= np.linspace(0, 1, n)
    signal[-n:] *= np.linspace(1, 0, n)
    return signal


def save(name, signal, gain=0.9):
    signal = np.asarray(signal, dtype=np.float64)
    peak = np.max(np.abs(signal)) or 1.0
    signal = fade_edges(signal / peak * gain)
    data = (signal * 32767).astype(np.int16)
    os.makedirs(OUT, exist_ok=True)
    path = os.path.join(OUT, name + ".wav")
    with wave.open(path, "wb") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(SR)
        w.writeframes(data.tobytes())
    print("wrote", os.path.normpath(path), f"{len(data) / SR:.2f}s")


def mix(*parts):
    n = max(len(p) for p in parts)
    out = np.zeros(n)
    for p in parts:
        out[: len(p)] += p
    return out


# --- Player -----------------------------------------------------------------
d = 0.16
save("PlayerShoot", (0.7 * sweep(1600, 280, d, "square") + 0.3 * noise(d)) * env(d, decay=22))

d = 0.3
save("PlayerHurt", (sweep(220, 90, d, "saw") * 0.8 + lowpass(noise(d), 0.2)) * env(d, decay=11))

d = 1.4
t = t_axis(d)
tremolo = 0.6 + 0.4 * np.sin(2 * np.pi * 9 * t)
save("PlayerDeath", mix(sweep(640, 45, d, "saw") * tremolo * env(d, 0.01, 2.2),
                        lowpass(noise(d), 0.08) * env(d, 0.01, 3.0) * 0.8))

# --- Enemies ----------------------------------------------------------------
d = 0.55
t = t_axis(d)
shimmer = sweep(180, 950, d) * (0.5 + 0.5 * np.sin(2 * np.pi * 30 * t))
save("EnemySpawn", (shimmer + 0.3 * sweep(360, 1900, d)) * np.sin(np.pi * t / d) ** 0.7)

d = 0.22
save("EnemyShoot", (0.6 * sweep(900, 140, d) + 0.4 * sweep(905, 142, d, "square")) * env(d, decay=14))

d = 0.35
whoosh = lowpass(noise(d), 0.12) * np.sin(np.pi * t_axis(d) / d)
thud = sweep(160, 50, 0.18) * env(0.18, decay=18)
save("MeleeAttack", mix(whoosh * 0.7, np.concatenate([np.zeros(int(SR * 0.12)), thud])))

d = 0.09
save("EnemyHit", (sweep(2400, 1200, d) * 0.6 + noise(d) * 0.4) * env(d, 0.001, 45))

d = 0.7
save("EnemyDeath", mix(lowpass(noise(d), 0.15) * env(d, 0.002, 6),
                       sweep(300, 40, d) * env(d, 0.002, 5) * 0.7))

# --- UI / game flow ------------------------------------------------------------
d = 0.06
save("UIClick", sweep(1300, 1100, d) * env(d, 0.001, 50), gain=0.7)

d = 0.5
save("Place", mix(np.sin(2 * np.pi * 660 * t_axis(d)) * env(d, 0.003, 7),
                  np.concatenate([np.zeros(int(SR * 0.12)),
                                  np.sin(2 * np.pi * 990 * t_axis(d - 0.12)) * env(d - 0.12, 0.003, 7)])))

notes = [523.25, 659.25, 783.99, 1046.5]
parts = []
for i, f in enumerate(notes):
    length = 0.9 if i == len(notes) - 1 else 0.18
    tone = (np.sin(2 * np.pi * f * t_axis(length)) + 0.3 * np.sin(4 * np.pi * f * t_axis(length)))
    parts.append(tone * env(length, 0.005, 3 if i == len(notes) - 1 else 10))
save("Victory", np.concatenate(parts))

# Ambient: a 16 s seamless drone. Every frequency completes a whole number of
# cycles in 16 s, so the loop point is inaudible.
d = 16.0
t = t_axis(d)
drone = (np.sin(2 * np.pi * 55 * t) * 0.5
         + np.sin(2 * np.pi * 82.5 * t) * 0.3 * (0.5 + 0.5 * np.sin(2 * np.pi * t / 8))
         + np.sin(2 * np.pi * 110.0625 * t) * 0.2
         + np.sin(2 * np.pi * 220 * t) * 0.08 * (0.5 + 0.5 * np.sin(2 * np.pi * t / 16)))
save("Ambient", drone, gain=0.6)
