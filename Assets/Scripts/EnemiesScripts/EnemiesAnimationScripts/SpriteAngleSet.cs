using UnityEngine;
using System;

/**
 * sprite set for a given angle range
 */
[Serializable]
public class SpriteAngleSet
{
   [SerializeField] private float minAngle;
   [SerializeField] private float maxAngle;
   [SerializeField] private Sprite[] flapSprites;
   
   public float MinAngle => minAngle;
   public float MaxAngle => maxAngle;
   
   public Sprite[] SpriteSet => flapSprites;
}
