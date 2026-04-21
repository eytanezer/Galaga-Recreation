using System;
using Scripts.EnemiesScripts.EnemiesAnimationScripts;
using State.States;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SpriteAngleResolver : MonoBehaviour
{
    [SerializeField] private SpriteAngleSet[] spriteAngleSets;
    private Vector2 _direction;
    private Vector3 _lastPosition;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private SpriteFlapper _spriteFlapper;
    private Sprite[] _defaultSprites;
    private bool _setFixedDirection;
    

    void Start()
    {
        // init
        _spriteFlapper = GetComponent<SpriteFlapper>();
        SetSpriteFromAngle(0);
        _defaultSprites = spriteAngleSets[0].SpriteSet;
        _setFixedDirection = false;
    }

    /**
     * late update to track movement
     * find the direction of movement and set sprite accordingly
     */
    private void LateUpdate()
    {
        if (_setFixedDirection)
        {
            SetSpriteFromAngle(0);
            return;
        }
        
        Vector3 movementDelta = transform.position - _lastPosition;
        Vector3 velocity = movementDelta / Time.deltaTime;
        _lastPosition = transform.position;

        // print(movementDelta);
        // print(velocity);
        float rawAngle = 0;

        if (velocity.magnitude > 0.001f) // moving
        {
            // calculate angle
            rawAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg; // -180 to +180
            float normalized360  = (rawAngle + 270) % 360; // 0 to 360
        
            SetSpriteFromAngle(normalized360);
            return;
        }
    }

    /**
     * set sprite based on angle
     */
    public void SetSpriteFromAngle(float angle)
    {
        foreach (var spriteAngleSet in spriteAngleSets)
        {
            if (angle >= spriteAngleSet.MinAngle && angle < spriteAngleSet.MaxAngle)
            {
                _spriteFlapper.setSpriteSet(spriteAngleSet.SpriteSet);
                return;
            }
        }
        
        // default to first set
        if (spriteAngleSets.Length > 0)
        {
            _spriteFlapper.setSpriteSet(_defaultSprites);
        }
    }

    public void SetFixedDirection(bool setFixedDirection)
    {
        _spriteRenderer.sortingOrder = setFixedDirection ? 2 : 0;
        _setFixedDirection = setFixedDirection;
    }
}
