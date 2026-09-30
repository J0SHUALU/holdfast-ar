using System;
using System.Collections.Generic;
using HoldfastAR.Core;
using UnityEngine;

namespace HoldfastAR.Audio
{
    /// <summary>
    /// Single owner of every AudioSource in the game (Singleton).
    ///  - one 2D source for UI and player sounds (PlayOneShot, so overlapping shots share one source)
    ///  - one looping 2D source for ambient music
    ///  - a small fixed pool of 3D sources for positional enemy sounds in AR space
    /// Enemies and bullets never carry their own AudioSource, which avoids duplicating components.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Serializable]
        private struct SoundSettings
        {
            public SoundId id;
            [Range(0f, 1f)] public float volume;
        }

        [SerializeField] private int spatialVoices = 6;
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 0.35f;
        [SerializeField] private SoundSettings[] volumeOverrides =
        {
            new SoundSettings { id = SoundId.PlayerShoot, volume = 0.55f },
            new SoundSettings { id = SoundId.EnemyShoot, volume = 0.7f },
            new SoundSettings { id = SoundId.UIClick, volume = 0.6f },
        };

        private readonly Dictionary<SoundId, AudioClip> _clips = new Dictionary<SoundId, AudioClip>();
        private readonly Dictionary<SoundId, float> _volumes = new Dictionary<SoundId, float>();
        private AudioSource _uiSource;
        private AudioSource _musicSource;
        private AudioSource[] _spatialSources;
        private int _nextSpatial;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            LoadClips();
            _uiSource = CreateSource("UI + Player (2D)", 0f);
            _musicSource = CreateSource("Music (2D loop)", 0f);
            _musicSource.loop = true;

            _spatialSources = new AudioSource[spatialVoices];
            for (int i = 0; i < spatialVoices; i++)
            {
                AudioSource s = CreateSource($"Spatial Voice {i}", 1f);
                s.minDistance = 0.3f;   // AR scale: enemies are metres, not tens of metres, away
                s.maxDistance = 6f;
                s.rolloffMode = AudioRolloffMode.Linear;
                _spatialSources[i] = s;
            }
        }

        private void LoadClips()
        {
            foreach (SoundId id in Enum.GetValues(typeof(SoundId)))
            {
                AudioClip clip = Resources.Load<AudioClip>("Audio/" + id);
                if (clip != null) _clips[id] = clip;
                else Debug.LogWarning($"AudioManager: missing clip Resources/Audio/{id}");
                _volumes[id] = 1f;
            }
            foreach (SoundSettings s in volumeOverrides) _volumes[s.id] = s.volume;
        }

        private AudioSource CreateSource(string sourceName, float spatialBlend)
        {
            var go = new GameObject(sourceName);
            go.transform.SetParent(transform, false);
            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = spatialBlend;
            return source;
        }

        /// <summary>Plays a non-positional sound (UI, player weapon, player damage).</summary>
        public void Play(SoundId id, float pitchVariation = 0f)
        {
            if (!_clips.TryGetValue(id, out AudioClip clip)) return;
            _uiSource.pitch = 1f + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
            _uiSource.PlayOneShot(clip, _volumes[id] * masterVolume);
        }

        /// <summary>Plays a sound at a world position using the next voice in the spatial pool.</summary>
        public void PlayAt(SoundId id, Vector3 position, float pitchVariation = 0.05f)
        {
            if (!_clips.TryGetValue(id, out AudioClip clip)) return;
            AudioSource source = _spatialSources[_nextSpatial];
            _nextSpatial = (_nextSpatial + 1) % _spatialSources.Length;

            source.transform.position = position;
            source.pitch = 1f + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
            source.PlayOneShot(clip, _volumes[id] * masterVolume);
        }

        public void PlayMusic(SoundId id)
        {
            if (!_clips.TryGetValue(id, out AudioClip clip)) return;
            if (_musicSource.clip == clip && _musicSource.isPlaying) return;
            _musicSource.clip = clip;
            _musicSource.volume = musicVolume * masterVolume;
            _musicSource.Play();
        }

        public void StopMusic() => _musicSource.Stop();
    }
}
