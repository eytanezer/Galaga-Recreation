// using System;
// using System.Collections;
// using UnityEngine;
//
// namespace Scripts.PlayerScripts
// {
//     public class PlayerAbduction : MonoBehaviour
//     {
//         public static event Action OnCaptureSequenceComplete;
//
//         [Header("Components")] [SerializeField]
//         private SpriteRenderer _spriteRenderer;
//
//         [SerializeField] private Collider2D _collider;
//
//         // reference to the player's script to disable it during abduction
//         [SerializeField] private MonoBehaviour playerMovementScript;
//         [SerializeField] private MonoBehaviour playerShootingScript;
//
//         [Header("Abduction Settings")] [SerializeField]
//         private float abductionDuration = 3.0f;
//
//         [SerializeField]
//         private float spriteChangeTime = 2f; // time after which the sprite changes to the abduction sprite
//
//         [SerializeField] private float spinSpeed = 360f; // degrees per second
//         [SerializeField] private Sprite abductionSprite;
//
//         public void StartAbduction(Transform target)
//         {
//             StartCoroutine(AbductionRoutine(target));
//         }
//
//         private IEnumerator AbductionRoutine(Transform target)
//         {
//             if (!target) yield return null;
//
//             if (playerMovementScript) playerMovementScript.enabled = false;
//             if (playerShootingScript) playerShootingScript.enabled = false;
//             if (_collider) _collider.enabled = false;
//
//             float elapsedTime = 0f;
//
//             Vector3 initialPosition = transform.position;
//             Quaternion initialRotation = transform.rotation;
//
//             while (elapsedTime < abductionDuration)
//             {
//                 elapsedTime += Time.deltaTime;
//                 float t = elapsedTime / abductionDuration;
//
//                 //move towards the target
//                 transform.position = Vector3.Lerp(initialPosition, target.position, t);
//
//                 // spin the player
//                 transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
//
//                 if (elapsedTime >= spriteChangeTime) _spriteRenderer.sprite = abductionSprite;
//
//                 yield return null;
//             }
//             
//             // snap to final state
//             transform.rotation = initialRotation; // Reset rotation
//         
//             // tell ENEMY, UI. HEALTH that the capture is done
//             OnCaptureSequenceComplete?.Invoke();
//         
//             // hide this player object so a new one can be spawned and the enemy can do its thing
//             gameObject.SetActive(false);
//         }
//     }
// }