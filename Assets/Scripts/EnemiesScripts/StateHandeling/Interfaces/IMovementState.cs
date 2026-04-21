namespace State.Interfaces
{
    /// <summary>
    /// Responsible for character movement 
    /// </summary>
    public interface IMovementState
    {
        void EnterState(IMovementStateContext context);
        void ExitState(IMovementStateContext context);
        void Update(IMovementStateContext context);
    }
}
