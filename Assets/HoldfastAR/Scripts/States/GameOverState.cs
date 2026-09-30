using HoldfastAR.Audio;
using HoldfastAR.Data;

namespace HoldfastAR.States
{
    /// <summary>End-of-match summary. Saves the session to the leaderboard exactly once.</summary>
    public class GameOverState : GameState
    {
        private readonly bool _survived;

        public GameOverState(GameManager game, bool survived) : base(game) => _survived = survived;

        public override string Name => _survived ? "Victory" : "Defeat";

        public override void Enter()
        {
            SessionRecord record = Game.CurrentSession.ToRecord(_survived);
            Game.Leaderboard.Add(record);
            Game.UI.ShowGameOver(record);
            if (_survived) AudioManager.Instance?.Play(SoundId.Victory);
        }
    }
}
