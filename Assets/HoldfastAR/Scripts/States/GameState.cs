namespace HoldfastAR.States
{
    /// <summary>
    /// Base class for the State pattern. Each state owns its own enter/update/exit
    /// logic, so GameManager never needs a big switch over "what mode are we in".
    /// </summary>
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
