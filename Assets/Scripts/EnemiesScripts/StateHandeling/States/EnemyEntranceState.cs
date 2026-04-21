using System;
using State.Interfaces;
using State.Models;
using UnityEngine;

namespace State.States
{
    /// <summary>
    /// Used to handle enemy ENTRANCE behavior
    /// </summary>
    public class EnemyEntranceState : IMovementState
    {
        private float _enterTime;

        // MOVEMENT STATE INTERFACE METHODS
        public void EnterState(IMovementStateContext context)
        {
            context.SetRotationLock(false);
            Debug.Log($"Enemy {context.EnemyTransform.name} starting spline: {context.EntranceSplineAnimator.Container}");
            
            if (context.EntranceSplineAnimator.Container == null)
            {
                Debug.LogError("EntranceSplineAnimator has no Container assigned!");
                return;
            }
            
            context.EntranceSplineAnimator.enabled = true;
            context.EntranceSplineAnimator.Restart(false);
            context.EntranceSplineAnimator.Play();
            
            Debug.Log($"Animation started - IsPlaying: {context.EntranceSplineAnimator.IsPlaying}");

        }

        public void ExitState(IMovementStateContext context)
        {
            context.EntranceSplineAnimator.NormalizedTime = 0f;
            context.EntranceSplineAnimator.enabled = false;
            
        }

        public void Update(IMovementStateContext context)
        {
            if (context.EntranceSplineAnimator.NormalizedTime < 1.0f) // if entrance animation is still playing
                return;
            
            
            // play after the entrance animation is complete
            var targetPosition = context.AssignedFormationSlot.TargetWorldPosition;
        
            // move towards the slot
            context.EnemyTransform.position = Vector3.MoveTowards(
                context.EnemyTransform.position, 
                targetPosition, 
                Time.deltaTime * context.EntranceSplineAnimator.MaxSpeed
            );

            // check if we are close enough to "lock in"
            if (Vector3.Distance(context.EnemyTransform.position, targetPosition) < 0.05f)
            {
                context.MovementData.IsIdle = true;
                context.MovementData.IsEntering = false;
            }
        }
    }
}