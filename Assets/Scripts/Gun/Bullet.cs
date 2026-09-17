using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _bulletVFX;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private float _knockbackThrust = 20f;

    private Vector2 _fireDirection;

    private Rigidbody2D _rigidBody;
    private Gun _gun;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // private void Start() {
    //     if (PlayerController.Instance.IsFacingRight()) {
    //         _fireDirection = Vector2.right;
    //     } else {
    //         _fireDirection = Vector2.left;
    //     }
    // }
    public void Init(Gun gun, Vector2 bulletSpawnPosition, Vector2 mousePosition)
    {
        _gun = gun;
        transform.position = bulletSpawnPosition;
        _fireDirection = (mousePosition - bulletSpawnPosition).normalized;
    }
    private void FixedUpdate()
    {
        _rigidBody.linearVelocity = _fireDirection * _moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other) {

        Instantiate(_bulletVFX, transform.position, Quaternion.identity);

        IHitable iHitable = other.gameObject.GetComponent<IHitable>();
        iHitable?.TakeHit();

        IDamagable iDamagable = other.gameObject.GetComponent<IDamagable>();
        iDamagable?.TakeDamage(_damageAmount, _knockbackThrust);

        _gun.ReleaseBulletFromPool(this);
    }
}