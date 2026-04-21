using State.Models;

namespace State.Interfaces
{
    public interface IMovementStateMachine
    {
        void UpdateState(MovementData movementData);
        void Update();
    }
}