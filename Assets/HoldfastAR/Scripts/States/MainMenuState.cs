using HoldfastAR.Audio;

namespace HoldfastAR.States
{
    /// <summary>Start screen: title, start, leaderboard and difficulty selection.</summary>
    public class MainMenuState : GameState
    {
        public MainMenuState(GameManager game) : base(game) { }

        public override string Name => "MainMenu";

        public override void Enter()
        {
            Game.Placement.CancelPlacement();
            Game.UI.ShowMainMenu();
            AudioManager.Instance?.PlayMusic(SoundId.Ambient);
        }
    }
}
