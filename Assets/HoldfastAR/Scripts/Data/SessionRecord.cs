using System;
using System.Collections.Generic;

namespace HoldfastAR.Data
{
    /// <summary>One finished play session, as stored in the leaderboard.</summary>
    [Serializable]
    public class SessionRecord
    {
        public string date;
        public string difficulty;
        public int score;
        public int enemiesDefeated;
        public float timeSurvived;
        public bool survived;
    }

    /// <summary>JsonUtility cannot serialise a bare list, so it is wrapped.</summary>
    [Serializable]
    public class SessionHistory
    {
        public List<SessionRecord> sessions = new List<SessionRecord>();
    }
}
