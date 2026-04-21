using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ManagmentScripts.SoundScripts;
using Scripts.Management;
using Scripts.Management.Formation;
using UnityEngine;

namespace Scripts.PlayerScriptes
{
    public class PlayerHitHandler : MonoBehaviour
    {
        private static readonly int Explode = Animator.StringToHash("PlayerExplode");
        public static event Action<bool> OnPlayerState;
        [SerializeField] private Animator anim;
        [SerializeField] private float respawnDelay = 2.0f;
        [SerializeField] private AudioClip playerExplosionSound;
        
        private Collider2D _playerCollider;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _startPosition;
        private PlayerMovment _playerMovement;
        
        [SerializeField] private float flickerDuration = 2.0f; // Total duration of the flicker effect
        [SerializeField] private float flickerInterval = 0.3f; // Time between blinks
        [SerializeField] private float invisibleAlpha = 0.2f;
        [SerializeField] private float visibleAlpha = 1.0f;
        private bool _isInvulnerable;

        private Sequence flickerSequence;
        
        public static event Action OnPlayerHit;

        private void Awake()
        {
            _playerCollider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _startPosition = transform.position;
            _playerMovement = GetComponent<PlayerMovment>();
        }


        private void OnEnable()
        {
            _isInvulnerable = false;
            PlayerHealth.OnPlayerDeath += CallDeathRoutine;
            if (SquadSpawner.Instance != null) SquadSpawner.Instance.OnAllEnemiesDead += CallDeathRoutine;

            ResetPlayer();
        }

        private void OnDisable()
        {
            PlayerHealth.OnPlayerDeath -= CallDeathRoutine;
            if (SquadSpawner.Instance != null) SquadSpawner.Instance.OnAllEnemiesDead -= CallDeathRoutine;

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (GameStateManager.Instance.CurrentState != GameState.Playing) return;
            
            Debug.Log("Player collided with: " + collision.gameObject.name);
            if (collision.gameObject.CompareTag("EnemyBullet"))
            {
                Debug.Log("Player hit by Enemy Bullet");
                
                OnHit();
            }
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Player hit by Enemy");
                
                OnHit();
            }
        }

        private void OnHit()
        {
            if(_isInvulnerable) return;
            
            _isInvulnerable = true;
            _playerCollider.enabled = false;
            anim.SetTrigger(Explode);
            _playerMovement.StopMovement(respawnDelay);
            SoundManager.Instance.PlaySoundFXClip(playerExplosionSound, transform, 1f);
            OnPlayerHit?.Invoke();
            OnPlayerState?.Invoke(false);
        }
        
        private void afterAnimationDestroy()
        {
            _spriteRenderer.enabled = false;
        }

        
        
        public IEnumerator RespawnRoutine()
        {
            // Hide the player during the wait (logic handled in HitHandler)
            yield return new WaitForSeconds(respawnDelay);
            transform.position = _startPosition;


            // Reset position and turn back on
            transform.position = _startPosition;
        
            // Trigger a re-enable method in your HitHandler
            ResetPlayer();
        }
        
        private void CallDeathRoutine()
        {
            StartCoroutine(DeathRoutine());
        }
        

        private IEnumerator DeathRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);
            gameObject.SetActive(false);
        }
        
        public void ResetPlayer()
        {
            _spriteRenderer.enabled = true;
            _playerCollider.enabled = true;
            OnPlayerState?.Invoke(true);
            anim.Rebind(); // Resets animator to idle state
            anim.Update(0f);
            StartFlicker();
        }
        
        private void StartFlicker()
        {
            // Ensure any existing sequence is killed before starting a new one
            if (flickerSequence != null)
            {
                flickerSequence.Kill();
            }

            // Calculate the number of loops needed based on total duration and interval
            // int loopCount = Mathf.RoundToInt(flickerDuration / (flickerInterval * 2)); // *2 for fade out and fade in

            flickerSequence = DOTween.Sequence();

            // Append a fade to invisible, then a fade back to visible
            flickerSequence.Append(_spriteRenderer.DOFade(invisibleAlpha, flickerInterval)) // Instant fade out
                .SetEase(Ease.InOutSine)
                .SetLoops(Mathf.RoundToInt(flickerDuration/flickerInterval), LoopType.Yoyo)
                .OnComplete(() => {
                    // Ensure the sprite is fully visible when the effect ends
                    _spriteRenderer.DOFade(visibleAlpha, 0.1f);
                    _isInvulnerable = false;
                }).SetId("flicker");
        }

        private void OnDestroy()
        {
            DOTween.Kill("flicker");
        }
    }
}