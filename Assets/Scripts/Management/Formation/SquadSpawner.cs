using System;
using System.Collections;
using System.Collections.Generic;
using State.Models;
using UnityEngine;

namespace Scripts.Management.Formation
{
    /// <summary>
    /// manages the squad spawning logic
    /// </summary>
    public class SquadSpawner : MonoSingleton<SquadSpawner>
    {
        public event Action OnAllEnemiesDead;
        
        
        [Header("Spawner Settings")]
        [SerializeField] private FormationManager formationManager;
        [SerializeField] private float spawnInterval = 5.0f;
        [SerializeField] private float spawnStartBuffer = 2.0f;
        [SerializeField] private float spawnDelta = 0.25f;
        private Dictionary<int, List<EnemyController>> _squadsByOrder = new Dictionary<int, List<EnemyController>>();
        
        private int _availableEnemies;
        private bool _isPlaying = false;
        private bool _isSpawningFinished = false;
        
        public  bool IsSpawningFinished => _isSpawningFinished;
        

        protected override void OnEnable()
        {
            base.OnEnable();
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged += HandleStateChange;
            Cheats.OnResetEnemiesSpawning += ResetEnemiesSpawning;
        }

        private void OnDisable()
        {
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged -= HandleStateChange;
            Cheats.OnResetEnemiesSpawning -= ResetEnemiesSpawning;
        }

        // private void Update()
        // {
        //     if (_isSpawningFinished && _availableEnemies <= 0)
        //     {
        //         _isPlaying = false;
        //         OnAllEnemiesDead?.Invoke();
        //         GameStateManager.Instance.EndGame(true);
        //         _isSpawningFinished = false;
        //     }
        // }

        private void HandleStateChange(GameState newState)
        {
            if (newState == GameState.Transition) CreateAllSquads();
            if(newState == GameState.Playing && !_isPlaying) StartCoroutine(SpawnWaveRoutine());
            if (newState != GameState.Playing && _isPlaying)
            {
                StopAllCoroutines();
                StartCoroutine(ClearAllEnemies());
            }
        }
        
        /*
         * clear all the enemies from the screen
         * used for endgame and for cheats
         */
        private IEnumerator ClearAllEnemies()
        {
            // stop any active spawning coroutines
            yield return new WaitForSeconds(spawnStartBuffer);

            // find all objects with your enemy tag and destroy them
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                Destroy(enemy);
            }
            
            _isPlaying = false;
            _isSpawningFinished = false;
            _squadsByOrder.Clear();
            _availableEnemies = 0;
        }

        /*
         * instantiate all the squads and assign them to the given order
         */
        private void CreateAllSquads()
        {
            _squadsByOrder.Clear();
            _availableEnemies = 0;
            
            for (int order = 1; order <= 5; order++)
            {
                List<FormationSlot> slotsToFill = formationManager.GetSlotsByOrder(order);
                
                if(slotsToFill == null || slotsToFill.Count == 0) continue;
                
                    Debug.Log("Creating Squad: " + order);
                    _squadsByOrder[order] = new List<EnemyController>();

                    int i = 1;
                    foreach (FormationSlot slot in slotsToFill)
                    {
                        EnemyController enemy = Instantiate(slot.enemyPrefab);
                        
                        enemy.gameObject.name = enemy.gameObject.name+"  : " + order + "_" + i;
                        enemy.AssignedFormationSlot = slot;
                        enemy.SetEntranceSpline(slot.EntranceSpline);
                        enemy.SetDiveSpline(slot.DiveSpline);
                        enemy.SetDanceSpline(slot.DanceSpline);
                        slot.EnemyInSlot = enemy;
                        
                        _squadsByOrder[order].Add(enemy);
                        _availableEnemies++;
                        i++;
                    }
            }
            
            DiveManager.Instance.SetAllEnemiesSpawned(false);
            DanceManager.Instance.SetAllEnemiesSpawned(false);
            
        }
        
        /**
         * sending the squads in order with the specific entrance type
         */
        private IEnumerator SpawnWaveRoutine()
        {
            yield return new WaitForSeconds(spawnStartBuffer);
            _isPlaying = true;

            // CASE 1: Wave 1 (Order 1) - Both squads go out together
            // Because they share the same Order ID, they are already in the same list.
            // To make them "go together", we can spawn them in pairs or with a very short delay.
            if (_squadsByOrder.ContainsKey(1))
            {
                Debug.Log("Wave 1: Simultaneous Squad Spawning");
                yield return StartCoroutine(SpawnSimultaneous(1));
            }

            yield return new WaitForSeconds(spawnInterval);

            // CASE 2: Wave 2 (Order 2) - Interchanged/Alternating
            if (_squadsByOrder.ContainsKey(2))
            {
                Debug.Log("Wave 2: Interchangeable Spawning");
                yield return StartCoroutine(SpawnInterchanged(2));
            }
            
            yield return new WaitForSeconds(spawnInterval);

            // CASE 3: Standard Spawning for the rest (Orders 3+)
            for (int i = 3; i <= 5; i++)
            {
                if (_squadsByOrder.ContainsKey(i))
                {
                    yield return StartCoroutine(SpawnStandard(i));
                    yield return new WaitForSeconds(spawnInterval);
                }
            }

            DiveManager.Instance.SetAllEnemiesSpawned(true);
            DanceManager.Instance.SetAllEnemiesSpawned(true);
            _isSpawningFinished = true;
        }
        
        
        // ENTRANCE LOGICS //
        private IEnumerator SpawnInterchanged(int order)
        {
            List<EnemyController> allEnemies = _squadsByOrder[order];
            int half = allEnemies.Count / 2;

            for (int i = 0; i < half; i++)
            {
                allEnemies[i].StartEntrance(); // Enemy from Squad A
                yield return new WaitForSeconds(spawnDelta); // Short gap
        
                allEnemies[i + half].StartEntrance(); // Enemy from Squad B
                yield return new WaitForSeconds(spawnDelta);
            }
        }
        
        private IEnumerator SpawnSimultaneous(int order)
        {
            List<EnemyController> allEnemies = _squadsByOrder[order];
            int half = allEnemies.Count / 2;

            // This assumes the two squads have an equal number of enemies
            for (int i = 0; i < half; i++)
            {
                // Spawn from the first squad (first half of list)
                allEnemies[i].StartEntrance(); 
        
                // Spawn from the second squad (second half of list)
                allEnemies[i + half].StartEntrance(); 
        
                yield return new WaitForSeconds(spawnDelta);
            }
        }

        private IEnumerator SpawnStandard(int order)
        {
            List<EnemyController> allEnemies = _squadsByOrder[order];
            foreach (EnemyController  enemy in allEnemies)
            {
                enemy.StartEntrance();
                yield return new WaitForSeconds(spawnDelta);
            } 
        }
        
        
        public void DestroyEnemy()
        {
            _availableEnemies--;
            CheckWinCondition();
        }
        
        private void CheckWinCondition()
        {
            if (_isSpawningFinished && _availableEnemies <= 0 && _isPlaying)
            {
                _isPlaying = false;
                OnAllEnemiesDead?.Invoke();
                
                if (GameStateManager.Instance.CurrentState == GameState.Playing)
                {
                    GameStateManager.Instance.EndGame(true);
                }
                
                _isSpawningFinished = false;
            }
        }

        private void ResetEnemiesSpawning()
        {
            StopAllCoroutines();

            if (DiveManager.Instance is not null)
            {
                DiveManager.Instance.ForceStopAllDives();
                DiveManager.Instance.SetBreak(false);
                DiveManager.Instance.SetAllEnemiesSpawned(false);
            }
            _isSpawningFinished = false; // to prevent win condition trigger

            StartCoroutine(ClearAndRestart());
        }
        
        private IEnumerator ClearAndRestart()
        {
            // Reuse your existing clear logic
            yield return StartCoroutine(ClearAllEnemies()); 
    
            // Restart spawning immediately
            CreateAllSquads();
            StartCoroutine(SpawnWaveRoutine());
        }
    }
}