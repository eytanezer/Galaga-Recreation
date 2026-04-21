// using System;
// using Scripts.PlayerScriptes;
// using UnityEngine;
//
// namespace Scripts.EnemiesScripts.EnemiesCombat
// {
//     public class TractorBeam : MonoBehaviour
//     {
//         public static event Action<Transform> OnPlayerCaught;
//
//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             if (other.CompareTag("Player"))
//             {
//                 PlayerAbduction abduction = other.GetComponent<PlayerAbduction>();
//                 if (abduction != null)
//                 {
//                     abduction.StartAbduction(transform.parent); // Pass the enemy as the target
//                 }
//             }
//         }
//     }
// }