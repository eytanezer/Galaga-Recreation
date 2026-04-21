using ManagmentScripts.SoundScripts;
using Scripts.Bullets;
using State.Interfaces;
using State.Models;
using UnityEngine;

namespace State.States
{
    /// <summary>
    /// Used to handle enemy DIVING behavior
    /// </summary>
    public class EnemyDiveState : IMovementState
    {
        private static float BOTTOM_SCREEN_LIMIT = -11f;
        private static float TOP_SCREEN_LIMIT = 11f;
        private bool _hasFired; // to ensure bullet is fired only once during the dive
        private bool _isReturning;
        private Vector3 _diveStartLocation;
        
        private float _distanceTraveled;
        private float _totalSplineLength;
        private float _moveSpeed;
        
        // MOVEMENT STATE INTERFACE METHODS
        public void EnterState(IMovementStateContext context)
        {
            _isReturning = false;
            _hasFired = false;

            _moveSpeed = context.DiveSplineAnimator.MaxSpeed;
            _diveStartLocation = context.EnemyTransform.position;
            _distanceTraveled = 0f;
            _totalSplineLength = context.DiveSplineAnimator.Container.CalculateLength();
            
            // go out of formation
            context.EnemyTransform.SetParent(null);
            DiveManager.Instance.RegisterDiveStart(context as EnemyController);
            SoundManager.Instance.PlaySoundFXClip(context.DivingSound, context.EnemyTransform, 1f);
        }

        public void ExitState(IMovementStateContext context)
        {
            context.DiveSplineAnimator.NormalizedTime = 0f;
            context.DiveSplineAnimator.enabled = false;
            _hasFired = false;
            DiveManager.Instance.RegisterDiveReturn(context as EnemyController);
        }

        public void Update(IMovementStateContext context) // called every frame during Dive state 
        {
            float t = 0;
            if (!_isReturning)
            {
                // calculate the movement of the enemy based on the spline
                // the enemy doesn't actually go over the spline but copy it shape and parameters to set the movement.
                _distanceTraveled += Time.deltaTime * _moveSpeed;
                t = Mathf.Clamp01(_distanceTraveled / _totalSplineLength);

                Vector3 localSplinePos = context.DiveSplineAnimator.Container.EvaluatePosition(t);
                context.EnemyTransform.position = _diveStartLocation + localSplinePos;

                if (context.EnemyTransform.position.y < BOTTOM_SCREEN_LIMIT)
                {
                    Vector3 wrapPos = context.EnemyTransform.position;
                    wrapPos.y = TOP_SCREEN_LIMIT;
                    
                    context.EnemyTransform.position = wrapPos;
                    _isReturning = true;
                }

                if (t >= 1f)
                {
                    _isReturning = true;
                }
            }
            else
            {
                // PHASE 2: Returning to the moving formation slot
                Vector3 targetPos = context.AssignedFormationSlot.TargetWorldPosition;
            
                context.EnemyTransform.position = Vector3.MoveTowards(
                    context.EnemyTransform.position, 
                    targetPos, 
                    Time.deltaTime * context.DiveSplineAnimator.MaxSpeed
                );

                // Re-lock into the slot
                if (Vector3.Distance(context.EnemyTransform.position, targetPos) < 0.1f)
                {
                    context.MovementData.IsDiving = false;
                }
            }

            // Shooting logic 
            // if (!_hasFired && context.EnemyTransform.position.y < 0f)
            if (!_hasFired && t >= 0.2)
            {
                if (context.EnemyBulletSpawner != null)
                {
                    context.EnemyBulletSpawner.SpawnBullet(context.ShootingDirection);
                    _hasFired = true;
                }
            }
        }
    }
}