namespace HoldfastAR.States
{
    /// <summary>
    /// Scan the floor and tap to place the arena. GameManager skips this state when the
    /// arena was already placed in an earlier round, so only one instance ever exists.
    /// </summary>
    public class PlacementState : GameState
    {
        public PlacementState(GameManager game) : base(game) { }

        public override string Name => "Placement";

        public override void Enter()
        {
            Game.Placement.ArenaPlaced += OnArenaPlaced;
            Game.Placement.BeginPlacement();
            Game.UI.ShowPlacement();
        }

        public override void Tick(float deltaTime)
        {
            Game.UI.Placement.SetPlaneCount(Game.Placement.TrackedPlaneCount);
        }

        public override void Exit()
        {
            Game.Placement.ArenaPlaced -= OnArenaPlaced;
            Game.Placement.CancelPlacement();
        }

        private void OnArenaPlaced(UnityEngine.Transform arena) => Game.BeginMatch();
    }
}
