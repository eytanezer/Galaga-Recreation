using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Scripts.UIScripts
{
    public class flashText : MonoBehaviour
    {
        [SerializeField] private float flashDuration;
        private TextMeshProUGUI _scoreText;
        

        private void Awake()
        {
            _scoreText = GetComponent<TextMeshProUGUI>();
            // _lastTime = Time.time;
        }

        private void OnEnable()
        {
            StartCoroutine(Flash());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator Flash()
        {
            var wait = new WaitForSeconds(flashDuration);
            
            while (true) 
            {
                yield return wait;
                
                _scoreText.enabled = !_scoreText.enabled;
            }
            
        }
    }
}