using System;
using Scripts.Management.Formation;
using State.Interfaces;
using State.States;
using UnityEngine;
using UnityEngine.Splines;

namespace State.Models
{
    public class EnemyController : MonoBehaviour, IMovementStateContext
    {
        // Spline Animators
        [Header("Spline Animators")]
        [SerializeField] private SplineAnimate _entranceSplineAnimator;
        [SerializeField] private SplineAnimate _diveSplineAnimator;
        [SerializeField] private SplineAnimate _danceSplineAnimator;
        [SerializeField] private AudioClip enemyDivingSound;
        
        public SplineAnimate EntranceSplineAnimator => _entranceSplineAnimator;
        public SplineAnimate DiveSplineAnimator => _diveSplineAnimator;
        public  SplineAnimate DanceSplineAnimator => _danceSplineAnimator;

        private SplineContainer _pendingDanceSpline;
        public SplineContainer PendingDanceSpline => _pendingDanceSpline;


        [Header("Points Given By Enemy")]
        [SerializeField] private int _pointsPerEnemy;
        public static Action<int> EnemyGotHit;

        // formation slot reference
        public AudioClip DivingSound => enemyDivingSound;
        public FormationSlot AssignedFormationSlot { get; set; }
        
        // IMovementStateContext implementation
        private MovementStateMachine _stateMachine;
        private MovementData _movementData;
        
        public Transform EnemyTransform => transform;
        

        public MovementData MovementData { get => _movementData; set => _movementData = value; }
        private SpriteAngleResolver _angleResolver;
        
        [Header("Enemy Type Settings")]
        [Tooltip("Check this if this enemy is Type 2 and should play the teleport sound")]
        [SerializeField] private bool isTeleportingEnemy;

        //getter so DiveManager can read it
        public bool IsTeleportingEnemy => isTeleportingEnemy;
        
        // Enemy Shooting
        public EnemyBulletSpawner EnemyBulletSpawner {get; private set;}
        private Transform _playerTransform;

        public Vector2 ShootingDirection
        {
            get
            {
                if (_playerTransform == null)
                {
                    return Vector2.down; // default direction if player not found
                }
                Vector2 direction = (_playerTransform.position - transform.position).normalized;
                return direction;
            }
        }

        private void Awake()
        {
            _movementData = new MovementData
            {
                IsWaiting = true
            };
            
            _stateMachine = new MovementStateMachine(this);
            _angleResolver = GetComponentInChildren<SpriteAngleResolver>();
            OnMovementDataChange();
        }

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) _playerTransform = player.transform;
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        private void OnEnable()
        {
            _movementData.OnDataChanged += OnMovementDataChange;
        }

        private void OnDisable()
        {
            _movementData.OnDataChanged -= OnMovementDataChange;
        }

        private void OnMovementDataChange()
        {
            _stateMachine.UpdateState(_movementData);
        }


        public void StopAllMovement()
        {
            // if not null - then
            if(_entranceSplineAnimator) _entranceSplineAnimator.enabled = false; 
            if(_diveSplineAnimator) _diveSplineAnimator.enabled = false;
            
            Debug.Log("Movement Systems Reset. Ready for next state.");
        }
        
        public void SetRotationLock(bool isIdle)
        {
            _angleResolver.SetFixedDirection(isIdle);
        }
        
        /**
         * destroy object after explosion animation is over
         */
        private void afterAnimationDestroy() 
        {
            EnemyGotHit?.Invoke(_pointsPerEnemy);
            Destroy(gameObject);
        }
        
        
        public void StartEntrance()
        {
            gameObject.SetActive(true);
            if (!EnemyBulletSpawner)
            {
                EnemyBulletSpawner = GetComponent<EnemyBulletSpawner>();
                if (!EnemyBulletSpawner)
                {
                    Debug.LogError($"{gameObject.name} - EnemyBulletSpawner not found!");
                }
            }
                
            EnemyBulletSpawner = GetComponent<EnemyBulletSpawner>();
            _movementData.IsWaiting = false;
            _movementData.IsEntering = true; // This will now trigger the state change
            _movementData.IsDancing = false;
            Debug.Log($"{gameObject.name} StartEntrance called");
            OnMovementDataChange();
        }
        
        public void SetEntranceSpline(SplineContainer spline)
        {
            if(_entranceSplineAnimator && spline)
            {
                _entranceSplineAnimator.Container = spline;

                try
                {
                    // Force the animator to acknowledge the new spline
                    if (spline.Spline != null && spline.Spline.Count > 0)
                    {
                        // Only snap if we have a valid spline
                        _entranceSplineAnimator.Restart(true);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{gameObject.name} - SetEntranceSpline failed: {e.Message}");
                }
                
            }
        }
        
        
        public void SetDiveSpline(SplineContainer spline)
        {
            if(_diveSplineAnimator && spline)
            {
                _diveSplineAnimator.Container = spline;
            }
            
        }
        
      
        
        
        public void SetDanceSpline(SplineContainer spline)
        {
            // just store the reference - don't assign it to the animator yet
            _pendingDanceSpline = spline;
            Debug.Log($"{gameObject.name} - Dance spline stored (not assigned to animator yet)");
        }

        public void ApplyDanceSpline()
        {
            if (_danceSplineAnimator && _pendingDanceSpline)
            {
                _danceSplineAnimator.enabled = false;
                _danceSplineAnimator.Container = _pendingDanceSpline;
                _danceSplineAnimator.NormalizedTime = 0f;
            }
        }
    }
}