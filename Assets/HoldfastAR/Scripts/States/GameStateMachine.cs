using HoldfastAR.Core;
using UnityEngine;

namespace HoldfastAR.States
{
    public class GameStateMachine
    {
        public GameState Current { get; private set; }

        public void ChangeState(GameState next)
        {
            if (next == null) return;
            Current?.Exit();
            Current = next;
            Debug.Log($"[Holdfast AR] State -> {next.Name}");
            Current.Enter();
            GameEvents.RaiseGameStateChanged(next.Name);
        }

        public void Tick(float deltaTime) => Current?.Tick(deltaTime);
    }
}
