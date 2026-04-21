using System;
using UnityEngine;

namespace Scripts.Bullets
{
    public class PlayerBullet : Bullet
    {
        public static event Action OnPlayerBulletDestroyed;
        
        protected override void HandleCollision(Collider2D collision)
        {
            // check if bullet hit wall or enemy - wall = out of bounds
            if (collision.CompareTag("Wall") || collision.CompareTag("Enemy"))
            {
                // gameObject.SetActive(false);
                PlayerBulletPool.Instance.Return(this);
                OnPlayerBulletDestroyed?.Invoke(); // decrease active bullet count
            }
        }
        
    }
}