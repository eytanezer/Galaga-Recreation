using System;
using State.Models;
using UnityEngine;
using UnityEngine.Splines;

namespace Scripts.Management.Formation
{
    //[Serializable]
    public class FormationSlot : MonoBehaviour
    {
        public bool IsOccupied { get; set; }
        public EnemyController enemyPrefab;
        public SplineContainer EntranceSpline;
        public SplineContainer DiveSpline;
        public SplineContainer DanceSpline;
        
        public EnemyController EnemyInSlot {get; set;}
        public Vector3 TargetWorldPosition => transform.position;

        // Visualizes the slot in the Editor Scene view
        private void OnDrawGizmos()
        {
            Gizmos.color = IsOccupied ? Color.red : Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(0.5f, 0.5f, 0.1f));
        }

        public void AssignSquadInfo(EnemyController enemyPrefab, SplineContainer EntranceSpline, SplineContainer DiveSpline, SplineContainer DanceSpline)
        {
            this.enemyPrefab = enemyPrefab;
            this.EntranceSpline = EntranceSpline;
            this.DiveSpline = DiveSpline;
            this.DanceSpline = DanceSpline;
            
        }
    }
}