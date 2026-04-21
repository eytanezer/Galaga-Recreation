using Scripts.Management.Formation;
using State.Models;
using UnityEngine;
using UnityEngine.Splines;

namespace State.Interfaces
{
    public interface IMovementStateContext
    {
        SplineAnimate EntranceSplineAnimator { get; }
        SplineAnimate DiveSplineAnimator { get; }
        SplineAnimate DanceSplineAnimator { get; }
        
        SplineContainer PendingDanceSpline { get; }
        void ApplyDanceSpline();
        
        MovementData MovementData { get; set; }

        Transform EnemyTransform { get; }
        EnemyBulletSpawner EnemyBulletSpawner { get; }
        Vector2 ShootingDirection { get; }
        AudioClip DivingSound { get;  }
        
        FormationSlot AssignedFormationSlot { get; set; }
        void StopAllMovement();
        void SetRotationLock(bool isIdle);
    }
}