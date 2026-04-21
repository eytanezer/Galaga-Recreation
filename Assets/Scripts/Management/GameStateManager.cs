using System;
using System.Collections;
using ManagmentScripts.SoundScripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Management
{
    /// <summary>
    /// manages the different state of the game
    /// </summary>
    public enum GameState { Intro, Menu, Transition, Playing, GameOver}
    
    public class GameStateManager : MonoSingleton<GameStateManager>
    {
        public event Action<GameState> OnStateChanged;
        
        private GameState _currentState;

        public GameState CurrentState
        {
            get => _currentState;
            private set
            {
                _currentState = value;
                
    
                // Handle UI visibility locally
                menuPanel.SetActive(value == GameState.Menu);
                transitionPanel.SetActive(value == GameState.Transition);
                gamePlayPanel.SetActive(value == GameState.Playing || value == GameState.Transition);
                gameOverPanel.SetActive(value == GameState.GameOver);
                
                OnStateChanged?.Invoke(value); // Tell everyone the state changed
            }
        }

        [Header("UI Panels")] 
        [SerializeField] private GameObject playerInstance;
        [SerializeField] private GameObject gamePlayPanel;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject transitionPanel;
        [SerializeField] private GameObject gameOverPanel;
        
        [Header("Panels Sounds")] 
        [SerializeField] private AudioClip menuSound;
        [SerializeField] private AudioClip transitionSound;
        
        [Header("End Game Transition Delay")] 
        [SerializeField] private float delayAfterGameOver = 4f;
        
        
        private TextMeshProUGUI _phaseText;
        
        private DefaultInputActions _inputActions;

        protected override void Awake()
        {
            base.Awake();
            _inputActions = new DefaultInputActions();
        }
        
        private void Start()
        {
            _phaseText = transitionPanel.GetComponentInChildren<TextMeshProUGUI>();
            StartCoroutine(IntroSequence());
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _inputActions.Player.Choose.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Player.Choose.Disable();
        }
        
        
        private void Update()
        {
            // Global "Enter" listener for the menu
            if (CurrentState == GameState.Menu && _inputActions.Player.Choose.IsPressed())
            {
                StartCoroutine(BeginGameTransition());
            }
        }

        private IEnumerator IntroSequence()
        {
            CurrentState = GameState.Intro;
            // TODO: Logic for your opening sequence (e.g., logo fade) goes here
            yield break;
        
            // GoToMenu();
        }

        public void GoToMenu()
        {
            // yield return null;
            CurrentState = GameState.Menu;
            menuPanel.SetActive(true);
            SoundManager.Instance.PlaySoundFXClip(menuSound, transform, 1f);
            transitionPanel.SetActive(false);
        }
        
        private IEnumerator BeginGameTransition()
        {
            menuPanel.SetActive(false);
            CurrentState = GameState.Transition;
        
            transitionPanel.SetActive(true);
            SoundManager.Instance.PlaySoundFXClip(transitionSound, transform, 1f);
            _phaseText.text = "PLAYER 1";
            yield return new WaitForSeconds(3f); // The "little gap"
            _phaseText.text = "STAGE 1";
            yield return new WaitForSeconds(2f);
            
            playerInstance.SetActive(true);

            CurrentState = GameState.Transition;
            
            _phaseText.text = "PLAYER 1\nSTAGE 1";
            yield return new WaitForSeconds(1f);
            
            StartGameplay();
            transitionPanel.SetActive(false);
        }
        
        private void StartGameplay()
        {
            // playerInstance.SetActive(true);
            gamePlayPanel.SetActive(true);
            StartCoroutine(activateGameplayAfterStart());

        }

        private IEnumerator activateGameplayAfterStart()
        {
            yield return new WaitForSeconds(2f);
            CurrentState = GameState.Playing;
        }        

        public void EndGame(bool isWin)
        {
            StartCoroutine(EndGameSequence(isWin));
        }

        private IEnumerator EndGameSequence(bool isWin)
        {
            
            yield return new WaitForSeconds(2f);
    
            CurrentState = GameState.GameOver; // This toggles your GameOver panel
    
            var endText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (isWin)
            {
                endText.text = "YOU WIN!";
            }
            else
            {
                endText.text = "GAME OVER";
            }
            SoundManager.Instance.StopAllSounds();
            yield return new WaitForSeconds(delayAfterGameOver);
            
            gameOverPanel.gameObject.SetActive(false); 
            yield return new WaitForSeconds(delayAfterGameOver*2);
            yield return null;
    
            GoToMenu(); // Return to the start screen
        }
    }
}