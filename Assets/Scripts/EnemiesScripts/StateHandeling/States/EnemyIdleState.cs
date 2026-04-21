using System;
using State.Interfaces;
using State.Models;
using UnityEngine;
using Random = System.Random;

namespace State.States
{
    /// <summary>
    /// Used to handle enemy IDLE behavior
    /// </summary>
    public class EnemyIdleState : IMovementState
    {
        
        
        // MOVEMENT STATE INTERFACE METHODS
        public void EnterState(IMovementStateContext context)
        {
            context.StopAllMovement();
            
            context.EnemyTransform.position = context.AssignedFormationSlot.TargetWorldPosition;

            context.EnemyTransform.SetParent(context.AssignedFormationSlot.transform);
            context.SetRotationLock(true);
            
            DiveManager.Instance.AddToIdlePool(context as EnemyController);
        }

        public void ExitState(IMovementStateContext context)
        {
            context.SetRotationLock(false);
            DiveManager.Instance.RemoveFromIdlePool(context as EnemyController);
        }

        public void Update(IMovementStateContext context)
        {
            
        }
    }
}