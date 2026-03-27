using System.Collections.Generic;
using UnityEngine;

public class SkillProjectileBase : MonoBehaviour
{
	private Rigidbody2D _rb;
	private ParticleSystem _projectile;
	private ParticleSystem _explosion;

	private Vector2 _direction;
	private Vector2 _startPosition;
	private float _speed = 30f;
	private float _range = 7f;
	private float _explosionX = 1;
	private float _explosionY = 1;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	public void Init(Vector2 direction, Vector2 startPosition, SkillData data, ParticleSystem
		projectileParticle, ParticleSystem explosionParticle = null)
	{
		_direction = direction;
		_startPosition = startPosition;
		_range = data.Range;
		_speed = data.ProjectileSpeed;
		_explosionX = data.DamageRangeX;
		_explosionY = data.DamageRangeY;

		// Root
		transform.right = _direction;
		transform.position = _startPosition;

		// Projectile Particle
		_projectile = Instantiate(projectileParticle, transform);
		_projectile.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		var main = _projectile.main;
		float remainTime = _range / _speed;
		main.startLifetime = remainTime;
		main.duration = remainTime;
		_projectile.Play();

		// Explosion Particle
		if (explosionParticle != null)
		{
			_explosion = Instantiate(explosionParticle, transform);
			explosionParticle.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		}
	}

	private void FixedUpdate()
	{
		_rb.MovePosition(_rb.position + _direction * (_speed * Time.fixedDeltaTime));

		if (Vector2.Distance(_startPosition, _rb.position) >= _range)
		{
			DestroyProjectile();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		DestroyProjectile();
	}

	private void DestroyProjectile()
	{
		// Physics
		ContactFilter2D contactFilter2D = ContactFilter2D.noFilter; // Todo: Set monster layer
		float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
		List<Collider2D> monsters = new();
		Physics2D.OverlapBox(transform.position, new Vector2(_explosionX, _explosionY), angle, contactFilter2D, monsters);

		foreach (Collider2D monster in monsters)
		{
			// monster.gameObject.GetComponent<?>().Hit();
			Destroy(monster.gameObject);
		}

		// Particle
		if (_explosion != null)
		{
			_explosion.transform.SetParent(null);
			_explosion.Play();
			Destroy(_explosion.gameObject, _explosion.main.duration + _explosion.main.startLifetime.constantMax);
		}

		Destroy(gameObject);
	}
}