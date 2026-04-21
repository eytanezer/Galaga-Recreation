using ManagmentScripts.SoundScripts;
using Scripts.EnemiesScripts.EnemiesAnimationScripts;
using Scripts.Management.Formation;
using State.Models;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    private static readonly int Explode = Animator.StringToHash("EnemyExplode");
    [SerializeField] private AudioClip enemyExplosionSound;
    // [SerializeField] private Animator anim;
    private EnemyController _enemyController;
    private SpriteFlapper _spriteFlapper;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Collider2D _collider2D;
    
    private void Awake()
    {
        _animator = GetComponentInParent<Animator>();
        _enemyController = GetComponentInParent<EnemyController>();
        _spriteFlapper = GetComponentInChildren<SpriteFlapper>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
    }
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enemy collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("PlayerBullet") || collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy hit by PlayerBullet");
            _enemyController.StopAllMovement();
            
            DiveManager.Instance.RegisterDiveReturn(_enemyController);
            DiveManager.Instance.RemoveFromIdlePool(_enemyController);
            
            _spriteFlapper.enabled = false;
            _spriteRenderer.enabled = false;
            
            _collider2D.enabled = false;
            _animator.SetTrigger(Explode);
            SoundManager.Instance.PlaySoundFXClip(enemyExplosionSound ,transform, 0.7f);
            SquadSpawner.Instance.DestroyEnemy();
            _enemyController.enabled = false;

        }
    }

    
}
