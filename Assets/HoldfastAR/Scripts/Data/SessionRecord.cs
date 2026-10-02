using System;
using System.Collections.Generic;

namespace HoldfastAR.Data
{
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

    [Serializable]
    public class SessionHistory
    {
        public List<SessionRecord> sessions = new List<SessionRecord>();
    }
}
