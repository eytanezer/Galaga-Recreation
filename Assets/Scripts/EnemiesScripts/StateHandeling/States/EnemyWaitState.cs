using State.Interfaces;

namespace State.States
{
    /// <summary>
    /// Used to handle enemy WAIT behavior
    /// </summary>
    public class EnemyWaitState : IMovementState
    {
        public void EnterState(IMovementStateContext context)
        {
            context.StopAllMovement();
            context.EnemyTransform.gameObject.SetActive(false);
        }

        public void ExitState(IMovementStateContext context)
        {
            context.EnemyTransform.gameObject.SetActive(true);
        }

        public void Update(IMovementStateContext context)
        {
            
        }
    }
}