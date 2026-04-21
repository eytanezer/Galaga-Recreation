using System;
using Scripts.Management;
using UnityEngine;

namespace Scripts.PlayerScriptes
{
    public class PlayerHealth : MonoBehaviour
    {
        public static event Action OnPlayerDeath;
        public static event Action<int>  UpdatePlayerHealth;
        [SerializeField] private int maxHealth;
        private int currentHealth;
        private PlayerHitHandler _playerHitHandler;

        private void Awake()
        {
            _playerHitHandler = gameObject.GetComponent<PlayerHitHandler>();
        }

        private void Start()
        {
            // currentHealth = maxHealth;
        }

        private void OnEnable()
        {
            currentHealth = maxHealth;
            UpdatePlayerHealth?.Invoke(currentHealth);
            PlayerHitHandler.OnPlayerHit += ReduceHealth;
            Cheats.OnResetPlayerHealth += ResetHealth;
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged += HandleStateChange;
        }

        private void OnDisable()
        {
            PlayerHitHandler.OnPlayerHit -= ReduceHealth;
            Cheats.OnResetPlayerHealth += ResetHealth;
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged += HandleStateChange;
        }

        private void ReduceHealth()
        {
            currentHealth -= 1;
            if (currentHealth <= 0)
            {
                GameStateManager.Instance.EndGame(false);
                OnPlayerDeath?.Invoke();
            }
            else
            {
                StartCoroutine(_playerHitHandler.RespawnRoutine());
            }
            
            UpdatePlayerHealth?.Invoke(currentHealth);
        }

        private void ResetHealth()
        {
            currentHealth = maxHealth;
            UpdatePlayerHealth?.Invoke(currentHealth);
        }
        
        private void HandleStateChange(GameState state)
        {
            if (state == GameState.Transition)
            {
                ResetHealth();
            }
        }
    }
}