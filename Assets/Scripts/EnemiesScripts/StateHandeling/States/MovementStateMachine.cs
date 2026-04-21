using State.Interfaces;
using State.Models;
using UnityEngine;

namespace State.States
{
    public class MovementStateMachine : IMovementStateMachine
    {
        private IMovementState _currentState = new EnemyEntranceState();
        private IMovementStateContext _context;

        public MovementStateMachine(IMovementStateContext context)
        {
            _context = context;
        }
        
        public void UpdateState(MovementData movementData)
        {
            IMovementState newState;
            if (movementData.IsWaiting)
            {
                newState = new EnemyWaitState();
            }
            else if (movementData.IsEntering)
            {
                newState = new EnemyEntranceState();
            }
            else if (movementData.IsDiving)
            {
                newState = new EnemyDiveState();
            }
            else if (movementData.IsDancing)
            {
                newState = new EnemyDanceState();
                Debug.Log("Dancing State assigned");
            }
            else
            {
                newState = new EnemyIdleState();
            }

            if (_currentState.GetType() != newState.GetType())
            {
                _currentState.ExitState(_context);
                _currentState = newState;
                _currentState.EnterState(_context);
            }
        }

        public void Update() => _currentState.Update(_context);
    }
}