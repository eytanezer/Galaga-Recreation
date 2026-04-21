using System;
using Scripts.UIScripts;
using State.Models;
using UnityEngine;

namespace Scripts.Management
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private int specialMechanicScoreTarget;
        private int _currentScore;
        private int _highScore;
        private bool _targetReached;
        
        public static event Action OnScoreTarget;
        public static event Action<int> OnHighScore;

        private void Awake()
        {
            _currentScore = 0;
        }

        private void Start()
        {
            _targetReached = false;
        }

        public void AddScore(int score)
        {
            _currentScore += score;
            Debug.Log("Current Score: "+_currentScore);
            handleHighScore(_currentScore);
            if (_currentScore >= specialMechanicScoreTarget && !_targetReached)
            {
                _targetReached = true;
                OnScoreTarget?.Invoke();
            }
        }

        private void OnEnable()
        {
            EnemyController.EnemyGotHit += AddScore;
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged += resetScore;
            
        }

        private void OnDisable()
        {
            EnemyController.EnemyGotHit -= AddScore;
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged -= resetScore;
            
        }

        private void resetScore(GameState state)
        {
            _currentScore = (state == GameState.Transition) ? 0 : _currentScore;
            _targetReached = false;
        }

        private void handleHighScore(int score)
        {
            if (score > _highScore)
            {
                _highScore = score;
                OnHighScore?.Invoke(_highScore);
            }
        }
        
    }
}