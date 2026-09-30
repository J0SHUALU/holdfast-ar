using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoldfastAR.Data
{
    /// <summary>
    /// Local leaderboard. Saves every finished session as JSON in PlayerPrefs, so it
    /// persists between app launches, and keeps only the latest 5 sessions (newest first).
    /// </summary>
    public class Leaderboard
    {
        public const int MaxEntries = 5;
        private const string PrefsKey = "HoldfastAR.Leaderboard.v1";

        private SessionHistory _history;

        public IReadOnlyList<SessionRecord> Sessions => _history.sessions;

        public Leaderboard() => Load();

        public void Add(SessionRecord record)
        {
            _history.sessions.Insert(0, record);
            if (_history.sessions.Count > MaxEntries)
                _history.sessions.RemoveRange(MaxEntries, _history.sessions.Count - MaxEntries);
            Save();
        }

        /// <summary>Index of the highest score among the stored sessions, or -1.</summary>
        public int BestIndex()
        {
            int best = -1;
            for (int i = 0; i < _history.sessions.Count; i++)
                if (best < 0 || _history.sessions[i].score > _history.sessions[best].score) best = i;
            return best;
        }

        public void Clear()
        {
            _history = new SessionHistory();
            Save();
        }

        private void Load()
        {
            string json = PlayerPrefs.GetString(PrefsKey, string.Empty);
            try
            {
                _history = string.IsNullOrEmpty(json) ? new SessionHistory() : JsonUtility.FromJson<SessionHistory>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Leaderboard data was unreadable and has been reset: {e.Message}");
                _history = null;
            }
            if (_history == null || _history.sessions == null) _history = new SessionHistory();
        }

        private void Save()
        {
            PlayerPrefs.SetString(PrefsKey, JsonUtility.ToJson(_history));
            PlayerPrefs.Save();
        }
    }
}
