using System;
using UnityEngine;

namespace Scripts.EnemiesScripts.EnemiesAnimationScripts
{
    /**
     * used to flap through a set of sprites at a given rate
     */
    public class SpriteFlapper : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] float flapRate = 2f;
        
        private Sprite[] _activeFlapSprites;
        private int _currentFlapIndex;
        private float _flapTimer;
        

        public void FixedUpdate()
        {
            handleFlapTimer();
        }

        /**
         * switches to the next sprite in the active flap sprite set based on the flap rate
         */
        private void handleFlapTimer() 
        {
            _currentFlapIndex = Mathf.FloorToInt(Time.time * flapRate) % _activeFlapSprites.Length;
            updateSprite();
        }

        // updates the sprite renderer to the current flap sprite
        private void updateSprite()
        {
            if (_activeFlapSprites != null)
            {
                spriteRenderer.sprite = _activeFlapSprites[_currentFlapIndex];
            }
        }

        /**
         * sets the active flap sprite set and resets the flap index
         */
        public void setSpriteSet(Sprite[] spriteSet)
        {
            if (_activeFlapSprites != spriteSet)
            {
                _activeFlapSprites = spriteSet;
                _currentFlapIndex = 0;
            }
            
            updateSprite();
            
        }
    }
}