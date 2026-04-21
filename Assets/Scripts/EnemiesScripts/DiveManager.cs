using System.Collections;
using System.Collections.Generic;
using ManagmentScripts.SoundScripts;
using Scripts.Management;
using Scripts.PlayerScriptes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace State.Models
{
    public class DiveManager : MonoSingleton<DiveManager>
    {
        [Header("Settings")] 
        [SerializeField] private float minDiveInterval = 3f;
        [SerializeField] private float maxDiveInterval = 6f;

        [Header("Teleport")] 
        [SerializeField] private AudioClip enemyTeleportingSound;

        private bool _playedTeleSound;
        private const float TeleSoundLength = 4;

        private List<EnemyController> _availableIdleEnemies = new List<EnemyController>();
        private List<EnemyController> _activeDivers = new List<EnemyController>();
        private bool _allEnemiesSpawned = false;
        private bool _isOnBreak = false;
        private bool _isPlayerDead;
        
        public bool IsOnBreak => _isOnBreak;
        
        
        private void Start()
        {
            StartCoroutine(DiveDecisionRoutine());
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            ScoreManager.OnScoreTarget += StopDive;
            if (DanceManager.Instance != null ) DanceManager.Instance.OnDanceFinish += StarStopDive;
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged += HandleStateChange;
            PlayerHealth.OnPlayerDeath += setPlayerDead;
        }

        private void OnDisable()
        {
            ScoreManager.OnScoreTarget -= StopDive;
            if (DanceManager.Instance != null ) DanceManager.Instance.OnDanceFinish -= StarStopDive;
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged -= HandleStateChange;
            PlayerHealth.OnPlayerDeath -= setPlayerDead;
        }

        
        public void SetAllEnemiesSpawned(bool value) => _allEnemiesSpawned = value;
        public void SetBreak(bool value) => _isOnBreak = value;
        private void setPlayerDead()
        {
            _isPlayerDead = true;
        }
        
        private IEnumerator DiveDecisionRoutine()
        {
            while (true)
            {
                // Wait for a random duration between dives
                yield return new WaitForSeconds(Random.Range(minDiveInterval, maxDiveInterval));
        
                // Only trigger if the coast is clear and spawning is done
                if (_activeDivers.Count == 0 && _allEnemiesSpawned &&!_isPlayerDead)
                {
                    TriggerRandomDiveGroup();
                }
            }
        }


        public void AddToIdlePool(EnemyController enemy)
        {
            if (!_availableIdleEnemies.Contains(enemy))
                _availableIdleEnemies.Add(enemy);
        }

        public void RemoveFromIdlePool(EnemyController enemy)
        {
            if (_availableIdleEnemies.Contains(enemy))
                _availableIdleEnemies.Remove(enemy);
        }


        private void TriggerRandomDiveGroup()
        {
            _availableIdleEnemies.RemoveAll(item => item == null);
            
            if (_availableIdleEnemies.Count == 0) return;
            if (_activeDivers.Count != 0) return;
            if(_isOnBreak) return;
            
            
            
            EnemyController leader = _availableIdleEnemies[Random.Range(0, _availableIdleEnemies.Count)];
            if (leader == null) return;
            
            List<EnemyController> potentialNeighbors = GetNeighborsByPhysics(leader);
            
            for (int i = 0; i < potentialNeighbors.Count; i++)
            {
                EnemyController temp = potentialNeighbors[i];
                int randomIndex = Random.Range(i, potentialNeighbors.Count);
                potentialNeighbors[i] = potentialNeighbors[randomIndex];
                potentialNeighbors[randomIndex] = temp;
            }
            
            List<EnemyController> diveGroup = new List<EnemyController> { leader };

            for (int i = 0; i < potentialNeighbors.Count && i < 2; i++)
            {
                diveGroup.Add(potentialNeighbors[i]);
            }

            foreach (var diver in diveGroup)
            {
                if (!_activeDivers.Contains(diver))
                {
                    diver.MovementData.IsDiving = true;
                    if (diver.IsTeleportingEnemy && !_playedTeleSound)
                    {
                        SoundManager.Instance.PlaySoundFXClip(enemyTeleportingSound,diver.EnemyTransform,0.5f, TeleSoundLength);
                        _playedTeleSound = true;
                    }
                }
            }
            _playedTeleSound = false;
        }

        public void RegisterDiveStart(EnemyController enemy)
        {
            if (!_activeDivers.Contains(enemy))
            {
                _activeDivers.Add(enemy);
            }
        }

        public void RegisterDiveReturn(EnemyController enemy)
        {
            if (_activeDivers.Contains(enemy))
            {
                _activeDivers.Remove(enemy);
            }
        } 
        
        private List<EnemyController> GetNeighborsByPhysics(EnemyController leader)
        {
            List<EnemyController> neighbors = new List<EnemyController>();
            Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            float checkDistance = 2.0f; // Adjust based on your grid spacing
            int enemyLayerMask = LayerMask.GetMask("Enemy");
            
            if (leader == null) return neighbors;

            foreach (Vector2 dir in directions)
            {
                // Start the ray slightly outside the leader to avoid self-collision
                Vector2 origin = (Vector2)leader.transform.position + dir * 0.6f;
                RaycastHit2D hit = Physics2D.Raycast(origin, dir, checkDistance, enemyLayerMask);
                
                
                Debug.DrawRay(origin, dir * checkDistance, Color.red, 1.0f);
                if (hit.collider != null)
                {
                    EnemyController neighbor = hit.collider.GetComponentInParent<EnemyController>();
                    if (neighbor != null && neighbor.MovementData.IsIdle)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }

            // Pick 2 random neighbors from the list of 4 possible directions
            Debug.Log($"neighbors hit: {neighbors.Count}");
            return neighbors; 
        }


        public bool HasActiveDivers()
        {
            _activeDivers.RemoveAll(enemy => enemy is null);
            
            if(_activeDivers.Count > 0) return true;
            return false;
        }
            
            

        private void StopDive()
        {
            StarStopDive(true);
        }
        
        private void ResumeDive()
        {
            StarStopDive(false);
        }

        private void StarStopDive(bool value)
        {
            _isOnBreak = value;
        }

        private void HandleStateChange(GameState newState)
        {
            if (newState == GameState.Menu || newState == GameState.Transition)
            {
                StopAllCoroutines();
                
                _activeDivers.Clear();
                _availableIdleEnemies.Clear();
                _allEnemiesSpawned = false;
                _isOnBreak = false;
                _isPlayerDead = false;

                if (newState == GameState.Transition)
                {
                    StartCoroutine(DiveDecisionRoutine());
                }
            }
        }
        
        public void ForceStopAllDives()
        {
            StopAllCoroutines();
            _activeDivers.Clear(); // Force clear the list
            
            StartCoroutine(DiveDecisionRoutine());
        }
    }
}