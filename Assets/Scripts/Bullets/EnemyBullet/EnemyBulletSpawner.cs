using Scripts.Bullets;
using UnityEngine;

/**
 * Spawns bullets for enemy ships
 */
public class EnemyBulletSpawner : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    
    
    /**
     * launch bullet in given direction - called by enemy while in Dive state
     */
    public void SpawnBullet(Vector2 direction)
    {
        Debug.Log("Spawning Bullet");
        Bullet bullet = EnemyBulletPool.Instance.Get();
        bullet.transform.position = transform.position;
        bullet.Launch(direction, bulletSpeed);
    }
    
}
