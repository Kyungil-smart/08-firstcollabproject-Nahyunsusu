using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    private Rigidbody2D _rb;
    private ParticleSystem _particle;
    private Vector2 _direction;
    private Vector2 _startPosition;
    private float _speed;
    private float _range;
    private int _damage;
    private float _damageRangeX; 
    private float _damageRangeY; 
    private bool _isInitialized = false;

    private void Awake() => _rb = GetComponent<Rigidbody2D>();

    public void Init(Vector2 direction, Vector2 startPosition, int damage,
                     float speed, float range, float damageRangeX, float damageRangeY,
                     ParticleSystem particle = null)
    {
        _direction = direction.normalized;
        _startPosition = startPosition;
        _damage = damage;
        _speed = speed;
        _range = range;
        _damageRangeX = damageRangeX;
        _damageRangeY = damageRangeY;

        transform.position = startPosition;
        transform.right = _direction;

        if (particle != null)
        {
            _particle = Instantiate(particle, transform);
            _particle.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _particle.Play();
        }
        _isInitialized = true;
    }

    private void FixedUpdate()
    {
        if (!_isInitialized) return;
        _rb.MovePosition(_rb.position + _direction * (_speed * Time.fixedDeltaTime));
        if (Vector2.Distance(_startPosition, _rb.position) >= _range) DestroyProjectile();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isInitialized || other.CompareTag("Enemy")) return;
        if (other.TryGetComponent(out Damageable damageable)) damageable.TakeDamage(_damage);
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        _isInitialized = false;
        if (_particle != null)
        {
            _particle.transform.SetParent(null);
            _particle.Stop();
            Destroy(_particle.gameObject, _particle.main.startLifetime.constantMax);
        }
        Destroy(gameObject);
    }
}