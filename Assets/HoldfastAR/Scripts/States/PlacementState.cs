namespace HoldfastAR.States
{
    /// <summary>
    /// Scan the floor and tap to place the arena. If the arena was already placed in an
    /// earlier round we skip straight to Playing, so only one instance ever exists.
    /// </summary>
    public class PlacementState : GameState
    {
        public PlacementState(GameManager game) : base(game) { }

        public override string Name => "Placement";

        public override void Enter()
        {
            if (Game.Placement.IsPlaced)
            {
                Game.BeginMatch();
                return;
            }
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
