using HoldfastAR.Audio;

namespace HoldfastAR.States
{
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
