using System;
using ManagmentScripts.SoundScripts;
using Scripts.Bullets;
using Scripts.PlayerScriptes;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Spawns bullets for player ship
 */
public class PlayerBulletSpawner : MonoBehaviour
{
    public static event Action OnPlayerShot;
    
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private int maxBulletCount = 3;
    
    [SerializeField] private AudioClip playerShootingSound;
    
    private int _bulletCount;
    private InputAction _shootAction;
    private DefaultInputActions _inputActions;
    private bool _canShoot;
    
    
    private void Awake()
    {
        _inputActions = new DefaultInputActions();
        _canShoot = true;
    }
    
    private void OnEnable()
    {
        _inputActions.Player.Fire.Enable();
        _inputActions.Player.Fire.performed += OnFire;
        PlayerBullet.OnPlayerBulletDestroyed += OnBulletDestroyed;
        PlayerHitHandler.OnPlayerState += setCanShoot;

    }
    
    private void OnDisable()
    {
        _inputActions.Player.Fire.performed -= OnFire;
        _inputActions.Player.Fire.Disable();
        PlayerBullet.OnPlayerBulletDestroyed -= OnBulletDestroyed;
        PlayerHitHandler.OnPlayerState -= setCanShoot;
    }

    /**
     * called by input system when fire button pressed - CURRENTLY BUTTON IS X
     */
    public void OnFire(InputAction.CallbackContext inputValue) 
    {
        if (_canShoot)
        {
            SpawnBullet();
            OnPlayerShot?.Invoke();
        }
    }
    
    private void SpawnBullet()
    {
        if (_bulletCount < maxBulletCount) // check bullet limit
        {
            PlayerBullet bullet = PlayerBulletPool.Instance.Get();
            bullet.transform.position = transform.position;
            bullet.Launch(transform.up , bulletSpeed);
            SoundManager.Instance.PlaySoundFXClip(playerShootingSound, transform, 1f);
            _bulletCount++;
        }
        Debug.Log("Bullet Count: " + _bulletCount);
    }
    
    private void OnBulletDestroyed() // callback from bullet when destroyed
    {
        // _bulletCount = Math.Max(0, _bulletCount - 1); // decrease
        _bulletCount--;
        Debug.Log("Bullet Destroyed. Current Bullet Count: " + _bulletCount);
    }

    private void setCanShoot(bool value)
    {
        _canShoot = value;
    }


}
