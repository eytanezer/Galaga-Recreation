using System.Collections;
using Scripts.Management;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject boundsObject;
    private Vector3 _startPosition;
    private Vector2 _moveInput;
    private InputAction _moveAction;
    private DefaultInputActions _inputActions;
    private Vector2 _movement;
    private bool _canMove;

    private void Awake()
    {
        _inputActions = new DefaultInputActions();
        _moveAction = _inputActions.Player.Move;
        _startPosition = transform.position;
    }
    
    void Start()
    {
        _canMove = true;
    }
    

    private void Update()
    {
        // get movement input
        if (!_canMove ||
            GameStateManager.Instance is null ||
            GameStateManager.Instance.CurrentState != GameState.Playing) return;
        if (boundsObject is null) return;
        
        
        _movement = _moveAction.ReadValue<Vector2>();
        
        // calculate clamped position >> message to Yan - THIS IS HOW I WRITE
        // calculate bounds
        float minX = boundsObject.transform.position.x + boundsObject.transform.localScale.x / 2;
        float maxX = (-1*boundsObject.transform.position.x) - boundsObject.transform.localScale.x / 2;
        
        // calculate next position and clamp it
        float nextX =  transform.position.x + (_movement.x * moveSpeed * Time.deltaTime);
        float clampedX = Mathf.Clamp(nextX, minX, maxX);
        
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
    
    public void StopMovement(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        _canMove = false;
        _movement = Vector2.zero;
        yield return new WaitForSeconds(duration);
        _canMove = true;
    }
    
    // cheats and more
    void OnEnable()
    {
        Cheats.OnResetPlayerPosition += ResetPosition;
        if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged += HandleStateChange;
        
        _inputActions.Player.Move.Enable();
    }
    
    void OnDisable()
    {
        Cheats.OnResetPlayerPosition -= ResetPosition;
        if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged -= HandleStateChange;
        
        _inputActions.Player.Move.Disable();
    }
    
    private void ResetPosition() 
    {
        transform.position = _startPosition;
    }
    
    private void HandleStateChange(GameState newState)
    {
        if (newState == GameState.Transition)
        {
            // Reset player for the new round
            ResetPosition();
            _canMove = false; // Stay still during "Phase 1"
        }
        else if (newState == GameState.Playing)
        {
            _canMove = true; // Start moving!
        }
    }

}
