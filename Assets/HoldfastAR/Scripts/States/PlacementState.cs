namespace HoldfastAR.States
{
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
