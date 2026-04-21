using DefaultNamespace;
using UnityEngine;
using DG.Tweening;

/**
 * base for bullets - Enemy and Player
 */
public abstract class Bullet : MonoBehaviour, IPoolable
{
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider;

    /**
     * Used for handling collision in child classes
     */
    protected abstract void HandleCollision(Collider2D other);

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }
    
    /**
     * part of IPoolable interface
     */
    public void Reset()
    {
        _rigidbody2D.linearVelocity = Vector2.zero;
        
        _collider.enabled = false;

        transform.DOKill();
        // StopAllCoroutines();
        gameObject.SetActive(true);
        
    }
    
    /**
     * launch the bullet in a direction with a speed  - called after getting from pool
     */
    public void Launch(Vector2 direction, float speed)
    {
        gameObject.SetActive(true);
        _collider.enabled = true;
        _rigidbody2D.linearVelocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision);
    }
}
