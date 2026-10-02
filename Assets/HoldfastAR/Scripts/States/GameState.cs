namespace HoldfastAR.States
{
    public abstract class GameState
    {
        protected readonly GameManager Game;

        protected GameState(GameManager game) => Game = game;

        public abstract string Name { get; }
        public virtual void Enter() { }
        public virtual void Tick(float deltaTime) { }
        public virtual void Exit() { }
    }
}
