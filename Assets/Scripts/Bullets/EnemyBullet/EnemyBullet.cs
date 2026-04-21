using UnityEngine;

namespace Scripts.Bullets
{
    public class EnemyBullet : Bullet
    {
        protected override void HandleCollision(Collider2D collision)
        {
            // check if bullet hit wall or enemy - wall = out of bounds
            if (collision.CompareTag("Wall") || collision.CompareTag("Player"))
            {
                gameObject.SetActive(false);
                EnemyBulletPool.Instance.Return(this);
            }
        }
    }
}