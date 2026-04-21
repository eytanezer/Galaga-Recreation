using System;
using State.Models;
using UnityEngine;

namespace Scripts.Management
{
    public class ShootingStats : MonoSingleton<ShootingStats>
    {
        private float _shotsFired;
        private float _enemiesHit;

        public float ShotsFired => _shotsFired;
        public float EnemiesHit => _enemiesHit;

        
        

        protected override void OnEnable()
        {
            base.OnEnable();
            EnemyController.EnemyGotHit += enemyHit;
            PlayerBulletSpawner.OnPlayerShot += shotsFired;
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged += HandleStateChange;
        }

        private void OnDisable()
        {
            EnemyController.EnemyGotHit -= enemyHit;
            PlayerBulletSpawner.OnPlayerShot -= shotsFired;
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged -= HandleStateChange;
        }

        private void shotsFired()
        {
            _shotsFired++;
        }

        private void enemyHit(int obj)
        {
            _enemiesHit++;
        }

        private void HandleStateChange(GameState state)
        {
            if (state == GameState.Playing)
            {
                _shotsFired = 0;
                _enemiesHit = 0;
            }
        }
        
        public float HitMissRatio()
        {
            if (_shotsFired <= 0) return 0f;
            return (_enemiesHit / _shotsFired) * 100f;
        }
    }
}