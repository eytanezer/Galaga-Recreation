using System;
using State.Interfaces;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace State.States
{
    /// <summary>
    /// Used to handle enemy DANCE behavior
    /// </summary>
    public class EnemyDanceState : IMovementState
    {
        private  bool _isFollowingSpline;
        private Vector3 _targetStartPoint;
        private bool _hasFired; // to ensure bullet is fired only once during the dive
        
        
        public void EnterState(IMovementStateContext context)
        {
            _isFollowingSpline = false;
            
            if (context.PendingDanceSpline == null)
            {
                Debug.LogError($"Enemy {context.EnemyTransform.name} has no pending Dance Spline!");
                return;
            }
            
            _targetStartPoint = context.PendingDanceSpline.EvaluatePosition(0);
            
            context.DanceSplineAnimator.enabled = false;
            
            // go out of formation
            context.EnemyTransform.SetParent(null);
        }

        public void ExitState(IMovementStateContext context)
        {
            context.DanceSplineAnimator.NormalizedTime = 0f;
            context.DanceSplineAnimator.enabled = false;

            _isFollowingSpline = false;
        }

        public void Update(IMovementStateContext context)
        {
            if (!_isFollowingSpline)
            {
                Debug.Log("going to dance");
                // PHASE 1: Travel to the Start of the Dance Spline
                context.DanceSplineAnimator.enabled = false;
                
                context.EnemyTransform.position = Vector3.MoveTowards(
                    context.EnemyTransform.position, 
                    _targetStartPoint, 
                    Time.deltaTime * context.DanceSplineAnimator.MaxSpeed
                );
                
                
                if (Vector3.Distance(context.EnemyTransform.position, _targetStartPoint) < 0.1f)
                {
                    _isFollowingSpline = true;
                    context.ApplyDanceSpline();
                    context.DanceSplineAnimator.enabled = true;
                    context.DanceSplineAnimator.Restart(true);
                    context.DanceSplineAnimator.Play();
                }
            }
            else
            {
                // PHASE 2: Following the dance spline
                if (context.DanceSplineAnimator.NormalizedTime >= 1f)
                {
                    // PHASE 3: Return to formation slot
                    Vector3 targetPos = context.AssignedFormationSlot.TargetWorldPosition;
            
                    context.EnemyTransform.position = Vector3.MoveTowards(
                        context.EnemyTransform.position, 
                        targetPos, 
                        Time.deltaTime * context.DanceSplineAnimator.MaxSpeed
                    );

                    // Re-lock into the slot
                    if (Vector3.Distance(context.EnemyTransform.position, targetPos) < 0.1f)
                    {
                        context.MovementData.IsDancing = false; // Returns to Idle/Formation
                    }
                }
                
                if (context.EnemyTransform.position.y < 0f)
                {
                    if (context.EnemyBulletSpawner is not null && !_hasFired )
                    {
                        context.EnemyBulletSpawner.SpawnBullet(context.ShootingDirection);
                        _hasFired = true;
                    }
                }
                else
                {
                    _hasFired = false;
                }
            }
        }
    }
}