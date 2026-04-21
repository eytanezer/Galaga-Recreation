// using State.Models;
// using UnityEngine;
//
// namespace Scripts.EnemiesScripts.EnemiesCombat
// {
//     public class CaptorMechanic : MonoBehaviour
//     {
//         [Header("Capture visuals")]
//         [SerializeField] private GameObject capturedFighterVisual;
//         [SerializeField] private SpriteAngleResolver capturedFighterResolver;
//     
//         [Header("Positions")]
//         [SerializeField] private Vector3 capturedPosBelow = new Vector3(0, -1f, 0); 
//         [SerializeField] private Vector3 capturedPosAbove = new Vector3(0, 1f, 0);
//
//         public bool HasCapturedFighter { get; private set; }
//
//         private EnemyController _enemyController;
//         private Vector3 _lastPosition;
//
//         private void Awake()
//         {
//             _enemyController = GetComponent<EnemyController>();
//         }
//
//         private void Start()
//         {
//             if(capturedFighterVisual) capturedFighterVisual.SetActive(false);
//         }
//
//         public void ReceiveCapturedPlayer()
//         {
//             HasCapturedFighter = true;
//             if(capturedFighterVisual) capturedFighterVisual.SetActive(false);
//             
//             setMode(false);
//
//             if (_enemyController) _enemyController.MovementData.IsDiving = false;
//         }
//
//         public void setMode(bool mode){}
//     }
// }