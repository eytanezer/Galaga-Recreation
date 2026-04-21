using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ManagmentScripts.SoundScripts;
using Scripts.Management;
using Scripts.Management.Formation;
using State.Models;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines;
using Random = System.Random;
using TMPEffects.Components;
using TMPro;


public class DanceManager : MonoSingleton<DanceManager>
    {
        public event Action<bool> OnDanceFinish;
        
        [Header("DanceBreak Lights")]
        [SerializeField] private List<Light2D> beamLights;
        [SerializeField] private List<Light2D> spotLights;
        [SerializeField] private float flashTiming;
        [SerializeField] private float rotationTiming;
        [SerializeField] private float rotationAngle;
        [SerializeField] private float danceBreaklength = 20f;
        [SerializeField] private float transitionDuration;
        
        [Header("DanceBreak Sounds")]
        [SerializeField] private AudioClip danceSound;
        [SerializeField ]private float soundLength = 36;
        
        [Header("DanceBreak Timing")]
        [SerializeField] private float textKillDelay = 4f;
        [SerializeField] private float staggerDelay = 0.25f;
        [SerializeField] private float squadDelay = 1.0f;
        
        [Header("UI Effects")]
        [SerializeField] private TMP_Text danceLabel;
        private TMPAnimator danceTMPAnimator;
        
        private bool _allEnemiesSpawned;
        private bool _danceBreak = false;
        private List<Quaternion> _initialRotations = new List<Quaternion>();
        private List<float> _targetRadii = new List<float>();
        private List<float> _targetIntensities = new List<float>();
        private float elpasdTime;
        

        public void SetAllEnemiesSpawned(bool value) => _allEnemiesSpawned = value;

        private void Start()
        {
            // Capture the direction they are pointing in the editor
            foreach (var light in beamLights)
            {
                if (light != null)
                    _initialRotations.Add(light.transform.rotation);
                _targetRadii.Add(light.pointLightOuterRadius); // Capture the "On" size
                _targetIntensities.Add(light.intensity); // Capture the "On" brightness
                
                // Set to zero immediately for game start
                light.pointLightOuterRadius = 0;
                light.intensity = 0;
                light.enabled = false;
            }
            foreach (var light in spotLights)
            {
                if (light != null)
                    _initialRotations.Add(light.transform.rotation);
                _targetRadii.Add(light.pointLightOuterRadius); // Capture the "On" size
                _targetIntensities.Add(light.intensity); // Capture the "On" brightness
                
                // Set to zero immediately for game start
                light.pointLightOuterRadius = 0;
                light.intensity = 0;
                light.enabled = false;
            }
        
            // Ensure they start off for the game start
            TurnOnOffLights(false);
            danceLabel.gameObject.SetActive(false);
            danceTMPAnimator = danceLabel.GetComponent<TMPAnimator>();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            ScoreManager.OnScoreTarget += CallHandleDanceBreakTrigger;
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged += HandleStateChange;
            Cheats.OnStartDance += ForceDanceStart;
        }

        private void OnDisable()
        {
            ScoreManager.OnScoreTarget -= CallHandleDanceBreakTrigger; 
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged -= HandleStateChange;
            Cheats.OnStartDance -= ForceDanceStart;
        }
        
        
        private void Update()
        {
            if (_danceBreak)
            {
                RotateBeams();
                FlashSpots();
            }

        }

        private void TurnOnOffLights(bool isActive)
        {
            foreach (var light in spotLights)
            {
                if (light is not null)
                    light.enabled = isActive;
            }
            
            foreach (var light in beamLights)
            {
                if (light is not null)
                {
                    light.enabled = isActive;
                    light.intensity = 0f; 
                }
                    
            }
        }

        private void FlashSpots()
        {
            foreach (var light in spotLights)
            {
                if(elpasdTime<flashTiming) elpasdTime += Time.deltaTime;
                else
                {
                    Random random = new Random();
                    if (random.Next(3) >= 1) light.enabled = !light.enabled;
                    elpasdTime = 0f;
                }
            }
        }

        private void RotateBeams()
        {
            float angle = Mathf.Sin(Time.time * rotationTiming) * rotationAngle;        
            for (int i = 0; i < beamLights.Count; i++)
            {
                if (beamLights[i] != null)
                {
                    beamLights[i].transform.rotation = _initialRotations[i] * Quaternion.Euler(0, 0, angle);
                }
            }
        }
        
        private IEnumerator TransitionLights(bool isActive)
        {
            _danceBreak = isActive;
            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / transitionDuration;
            
                // If turning off, we want to go from 1 to 0
                float lerpVal = isActive ? t : 1f - t;
                // Use an EaseOut curve for a "pop" effect
                float curve = Mathf.Sin(lerpVal * Mathf.PI * 0.5f);

                for (int i = 0; i < beamLights.Count; i++)
                {
                    if (beamLights[i] == null) continue;
                    beamLights[i].pointLightOuterRadius = _targetRadii[i] * curve;
                    beamLights[i].intensity = _targetIntensities[i] * curve;
                }
                // yield return null;
                for (int i = beamLights.Count; i < spotLights.Count; i++)
                {
                    if (spotLights[i] == null) continue;
                    spotLights[i].pointLightOuterRadius = _targetRadii[i] * curve;
                    spotLights[i].intensity = _targetIntensities[i] * curve;
                }
                yield return null;
            }

            // Clean up: disable components if turning off to save performance
            if (!isActive)
            {
                foreach (var light in beamLights) light.enabled = false;
                foreach (var light in spotLights) light.enabled = false;
            }
        }

        private IEnumerator TriggerOrderedDance(int squadOrder, SplineContainer danceSpline)
        {
            // yield return new WaitForSeconds(4f);
            Debug.Log("Dancing order: "+ squadOrder);
            
            List<FormationSlot> squadSlots = FormationManager.Instance.GetSlotsByOrder(squadOrder);
            if (squadSlots == null || squadSlots.Count == 0) yield break;
            Debug.Log("Dancing order: "+ squadOrder);
            
            Vector3 splineStart = danceSpline.EvaluatePosition(0);

            // sort enemies by distance (Furthest first)
            var sortedEnemies = squadSlots
                .Where(slot => slot.EnemyInSlot != null)
                .Select(slot => slot.EnemyInSlot)
                .OrderBy(enemy => Vector3.Distance(enemy.transform.position, splineStart))
                .ToList();

            // trigger them one by one with a 0.25s delay
            foreach (var enemy in sortedEnemies)
            {
                if (enemy.MovementData.IsIdle && !enemy.MovementData.IsEntering)
                {
                    // enemy.SetDanceSpline(danceSpline); 
                    enemy.MovementData.IsDancing = true; // trigger state change
                }
        
                yield return new WaitForSeconds(staggerDelay);
            }
        }

 
        
        private void HandleStateChange(GameState newState)
        {
            if (newState == GameState.Transition)
            {
                _danceBreak = false;
                TurnOnOffLights(false);
            }

            if (newState == GameState.Menu)
            {
                StartCoroutine(TransitionLights(false));
                TurnOnOffLights(false);
                StopAllCoroutines();
                OnDanceFinish?.Invoke(false);
            }
            
        }
        

        private void CallHandleDanceBreakTrigger()
        {
            StartCoroutine(HandleDanceBreakTrigger());
        }

        private IEnumerator HandleDanceBreakTrigger()
        {
            // wait for current divers to finish before starting the dance
            while (!_allEnemiesSpawned)
            {
                yield return null;
            }

            TurnOnOffLights(true);
            SoundManager.Instance.PlaySoundFXClip(danceSound, transform, 1f, soundLength);
            
            _danceBreak = true;
            StartCoroutine(TransitionLights(true));
            
            if (danceLabel is not null)
            {
                danceLabel.gameObject.SetActive(true);
                // string coolText = "<wave >\n<palette> \nLET'S DANCE!\n</palette> \n</wave>";
                StartCoroutine(KillText());
            }
            
            StartCoroutine(WaitAndStartDance());
        }

        private IEnumerator WaitAndStartDance()
        {
            Debug.Log("Waiting for Dance Break Trigger");
            // wait while any enemies are still in the 'ActiveDivers' list
            while (DiveManager.Instance.HasActiveDivers()) 
            {
                yield return null; 
            }
            Debug.Log("Dance Break is ready");
            
            
            // trigger the squads one by one
            for (int i = 1; i <= 5; i++)
            {
                // trigger the ordered dance for this squad
                yield return StartCoroutine(TriggerOrderedDance(i, FormationManager.Instance.GetSlotsByOrder(i)[0].DanceSpline));
        
                yield return new WaitForSeconds(squadDelay);
            }
            
            yield return new WaitForSeconds(danceBreaklength);
            StartCoroutine(TransitionLights(false));
            // TurnOnOffLights(false);
            _danceBreak = false;
            OnDanceFinish?.Invoke(false);
        }

        private void ForceDanceStart()
        {
            _allEnemiesSpawned = true;
            DiveManager.Instance.ForceStopAllDives();
            DiveManager.Instance.SetAllEnemiesSpawned(true);
            OnDanceFinish?.Invoke(true);
            
            StopAllCoroutines();
            StartCoroutine(HandleDanceBreakTrigger());
        }

        private IEnumerator KillText()
        {
            yield return new WaitForSeconds(textKillDelay);
            danceLabel.gameObject.SetActive(false);
        }
    }
