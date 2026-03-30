using System.Collections.Generic;
using _Scripts.LYC.Skill;
using UnityEngine;

public class EnemySkillProjectile : SkillProjectile
{
	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	public void EnemyInit(Vector2 direction, Vector2 startPosition, EnemySkillExecutor executor,
		ParticleSystem projectileParticle,
		ParticleSystem explosionParticle = null)
	{
		SkillData skillData = executor.CurrentSkillData;
		EnemyData data = executor.Controller.data;

		_direction = direction;
		_startPosition = startPosition + direction;
		_range = skillData.Range;
		_speed = skillData.ProjectileSpeed;
		_explosionX = skillData.DamageRangeX;
		_explosionY = skillData.DamageRangeY;

		// Damage
		_skillDamage = skillData.Damage + data.attackDamage;

		// Root
		transform.right = _direction;
		transform.position = _startPosition;

		// Projectile Particle
		_projectile = Instantiate(projectileParticle, transform);
		_projectile.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		ParticleSystem.MainModule main = _projectile.main;
		float remainTime = _range / _speed + 0.2f; // 자연스러운 스킬 표현을 위한 오프셋
		main.startLifetime = remainTime;
		main.duration = remainTime;
		_projectile.Play();

		// Explosion Particle
		if (explosionParticle != null)
		{
			_explosion = Instantiate(explosionParticle, transform);
			float scale = Mathf.Max(_explosionX, _explosionY);
			_explosion.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			_explosion.transform.localScale = Vector3.one * scale;
		}
	}

	private void FixedUpdate()
	{
		Vector2 position = transform.position;
		_rb.MovePosition(position + _direction * (_speed * Time.fixedDeltaTime));

		if (Vector2.Distance(_startPosition, position) >= _range)
		{
			DestroyProjectile();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		// TODO: 벽 충돌 처리 필요

		if (other.TryGetComponent(out PlayerController p))
		{
			DestroyProjectile();
		}
	}

	protected override void DestroyProjectile()
	{
		// Physics
		ContactFilter2D contactFilter2D = ContactFilter2D.noFilter; // Todo: Set Player layer
		float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
		List<Collider2D> result = new();
		Physics2D.OverlapBox(transform.position, new Vector2(_explosionX, _explosionY), angle, contactFilter2D,
			result);

		foreach (Collider2D c in result)
		{
			if (!c.TryGetComponent(out PlayerController player)) continue;

			player.TakeDamage(_skillDamage);
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