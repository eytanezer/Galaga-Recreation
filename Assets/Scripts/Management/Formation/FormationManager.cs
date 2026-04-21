using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using State.Models;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public enum SquadShape { Square, Vertical, Horizontal }

namespace Scripts.Management.Formation
{
    /// <summary>
    /// manages the formation-movement and formation-logic of the enemies
    /// </summary>
    public class FormationManager : MonoSingleton<FormationManager>
    {
        [Header("Movement Settings")]
        [SerializeField] private float swayAmplitude = 2.5f;
        [SerializeField] private float swaySpeed = 0.8f;
        
        [Header("Breathing Settings")]
        [SerializeField] private float scaleAmount = 1.1f; // How much it expands
        [SerializeField] private float cycleTime = 2f;    // Speed of breath

        [Header("Grid Settings")]
        [SerializeField] private FormationSlot slotPrefab;
        [SerializeField] private float unitSpacing = 1.2f;

        [Header("Level Layout")] 
        [SerializeField] private List<SquadData> levelSquads;
        
        private Vector3 _startPosition;
        private Dictionary<int, List<FormationSlot>> _orderedSlots = new Dictionary<int, List<FormationSlot>>();
        
        private bool _isBreathing = false;
        private bool _isSwaying = false;
        private FormationSlot[] _slots;
        private Vector3[] _originalPositions;
        
        protected override void Awake()
        {
            base.Awake();
            _startPosition = transform.position;
            GenerateFormationGrid(); // Example: 5 rows and 5 columns
            
            _slots = GetComponentsInChildren<FormationSlot>();
            _originalPositions = new Vector3[_slots.Length];
        
            for (int i = 0; i < _slots.Length; i++)
            {
                _originalPositions[i] = _slots[i].transform.localPosition;
            }
            
            _isBreathing = false;
        }
        
        protected override void OnEnable()
        {
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged += HandleStateChange;
            
        }

        private void OnDisable()
        {
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged -= HandleStateChange;
            StopBreathing();
        }


        private void Update()
        {
            if (GameStateManager.Instance.CurrentState == GameState.Playing)
            {
                if (!_isSwaying)
                {
                    StartSwaying();
                }
                
                if(!_isBreathing && SquadSpawner.Instance.IsSpawningFinished)
                {
                    StartBreathing();
                }
            }
                
        }
        
        private void StartSwaying()
        {
            _isSwaying = true;
            transform.DOKill();
            float leftEdge = _startPosition.x - swayAmplitude;
            float rightEdge = _startPosition.x + swayAmplitude;

            //snap to the far left immediately so the loop is centered
            transform.position = new Vector3(leftEdge, transform.position.y, transform.position.z);
            
            transform.DOMoveX(rightEdge, swaySpeed)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo)
                .SetId("sway");
        }
        
        private void StartBreathing()
        {
            transform.DOKill();
            
            _isBreathing = true;

            transform.DOMoveX(_startPosition.x, swaySpeed)
                .SetEase(Ease.Linear)
                .SetId("moveToCenter");
                
            // Scale the whole container up and down infinitely
            DOTween.To(() => 1f, x => UpdateSlotSpacing(x), scaleAmount, cycleTime)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetId("breath");
        }
        
        private void UpdateSlotSpacing(float currentMultiplier)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                // Move the slot position away from the center (0,0,0) based on the multiplier
                _slots[i].transform.localPosition = _originalPositions[i] * currentMultiplier;
            }
        }
        

        private void GenerateFormationGrid()
        {
            foreach (var squad in levelSquads)
            {
                CreateSquad(squad);
            }
        }
        
        private void CreateSquad(SquadData data)
        {
            Vector2[] offsets = GetOffsetByShape(data.shape);
            
            if(!_orderedSlots.ContainsKey(data.squadOrder))
                _orderedSlots[data.squadOrder] = new List<FormationSlot>();

            foreach (var offset in offsets)
            {
                Vector3 pos = (Vector3)data.centerOffset + (Vector3)(offset * unitSpacing);
                
                FormationSlot slotObj = Instantiate(slotPrefab, transform);
                slotObj.transform.localPosition = pos;
                slotObj.name = $"Slot_{data.name}";
                
                slotObj.AssignSquadInfo(data.enemyPrefab, data.EntranceSpline, data.DiveSpline, data.DanceSpline);
                _orderedSlots[data.squadOrder].Add(slotObj);
            }
        }
        
        private Vector2[] GetOffsetByShape(SquadShape shape)
        {
            return shape switch
            {
                SquadShape.Square => 
                    new Vector2[] { new(-0.5f, 0.5f), new(0.5f, 0.5f), new(-0.5f, -0.5f), new(0.5f, -0.5f) },
                SquadShape.Vertical =>
                    new Vector2[] {new(0, 0.5f), new(0, -0.5f)},
                SquadShape.Horizontal =>
                    new Vector2[] { new(-1.5f, 0), new(-0.5f, 0), new(0.5f, 0), new(1.5f, 0) },
                _ => new Vector2[0]
            };
        }
        
        public List<FormationSlot> GetSlotsByOrder(int order) => _orderedSlots.GetValueOrDefault(order);
        
       
        private void HandleStateChange(GameState state)
        {
            // If the game ends or returns to menu, stop the effect
            if (state != GameState.Playing)
            {
                StopBreathing();
                _isSwaying = false;
            }
        }
        
        private void StopBreathing()
        {
            DOTween.Kill("moveToCenter");
            DOTween.Kill("breath");
            UpdateSlotSpacing(1);
            transform.position = _startPosition;
            _isBreathing = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            transform.DOKill();
        }
    }
}