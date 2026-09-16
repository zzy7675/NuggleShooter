using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
    public static Action OnShoot;

    private Coroutine _muzzleFlashRoutine;
    private ObjectPool<Bullet> _bulletPool;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _gunFireCD = 0.5f;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private float _muzzleFlashTime = .05f;

    private static readonly int FIRE_HASH = Animator.StringToHash("Fire");
    private float _lastFireTime = 0f;
    private Vector2 _mousePosition;

    private Animator _animator;
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        CreateBulletPool();
    }

    private void Update()
    {
        Shoot();
        RotateGun();
    }

    private void OnEnable() {
        OnShoot += ShootProjectile;
        OnShoot += ResetLastFireTime;
        OnShoot += FireAnimation;
        OnShoot += GunScreenShake;
        OnShoot += MuzzleFlash;
    }

    private void OnDisable() {
        OnShoot -= ShootProjectile;
        OnShoot -= ResetLastFireTime;
        OnShoot -= FireAnimation;
        OnShoot -= GunScreenShake;
        OnShoot -= MuzzleFlash;
    }

    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }

    private void CreateBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(
            () =>
            {
                return Instantiate(_bulletPrefab);
            },
            bullet =>
            {
                bullet.gameObject.SetActive(true);
            },
            bullet =>
            {
                bullet.gameObject.SetActive(false);
            },
            bullet =>
            {
                Destroy(bullet);
            }
            ,
            false,
            20,
            40
        );
    }

    private void Shoot()
    {
        if (Input.GetMouseButton(0) && Time.time >= _lastFireTime) {
            OnShoot?.Invoke();
        }
    }

    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time + _gunFireCD;
    }

    private void ShootProjectile()
    {
        Bullet newBullet = _bulletPool.Get();
        newBullet.Init(this, (Vector2)_bulletSpawnPoint.position, _mousePosition);
    }

    private void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0f);
    }

    private void GunScreenShake()
    {
        _impulseSource.GenerateImpulse();
    }

    private void RotateGun()
    {
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = PlayerController.Instance.transform.InverseTransformPoint(_mousePosition);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void MuzzleFlash()
    {
        if (_muzzleFlashRoutine != null)
        {
            StopCoroutine(_muzzleFlashRoutine);
        }

        _muzzleFlashRoutine = StartCoroutine(MuzzleFlashRoutine());
    }

    private IEnumerator MuzzleFlashRoutine()
    {
        _muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(_muzzleFlashTime);
        _muzzleFlash.SetActive(false);
    }
}
