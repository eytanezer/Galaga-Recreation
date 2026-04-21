using UnityEngine;

namespace Scripts.Management
{
    public class BackgroundLooper : MonoBehaviour
    {
        [Header("Movement Settings")] [SerializeField]
        private float scrollSpeed = 2f;

        [SerializeField] private float backgroundHeight = 16f; // Total height of your sprite
        [SerializeField] private bool autoStart;
        private bool _isGameStarted = false;
        private Vector3 _startPos;

        void Start()
        {
            _startPos = Vector3.zero;
            _isGameStarted = (autoStart) ? true : false;
        }

        void Update()
        {
            if (!_isGameStarted) return;

            // Move the background down
            transform.Translate(Vector2.down * scrollSpeed * Time.deltaTime);

            // If it moves past the threshold, snap it back up to the top of the "stack"
            if (transform.position.y <= _startPos.y - (backgroundHeight))
            {
                transform.position = new Vector3(transform.position.x, _startPos.y + backgroundHeight,
                    transform.position.z);
            }
        }
        
        private void OnEnable()
        {
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged += HandleStateChange;
        }

        private void OnDisable()
        {
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged -= HandleStateChange;
        }

        private void HandleStateChange(GameState newState)
        {
            if (newState == GameState.Intro){SetScrolling(false);}
            if (newState == GameState.Transition){SetScrolling(false);}
            if (newState == GameState.Playing){SetScrolling(true);}
            if (newState == GameState.Menu){SetScrolling(false);}
        }
        

        // Call this when the player clicks "Start Game"
        public void SetScrolling(bool value) => _isGameStarted = value;
    }
}