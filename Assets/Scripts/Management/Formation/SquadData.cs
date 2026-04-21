using State.Models;
using UnityEngine;
using UnityEngine.Splines;

namespace Scripts.Management.Formation
{
    [System.Serializable]
    public class SquadData
    {
        public string name;
        public SquadShape shape;
        public Vector2 centerOffset;
        public int squadOrder;

        public EnemyController enemyPrefab;
        public SplineContainer EntranceSpline;
        public SplineContainer DiveSpline;
        public SplineContainer DanceSpline;
    }
}