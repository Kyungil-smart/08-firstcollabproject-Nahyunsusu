using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.LYC.Skill
{
	public class SkillProjectile : MonoBehaviour
	{
		protected Rigidbody2D _rb;
		protected ParticleSystem _projectile;
		protected ParticleSystem _explosion;

		protected Vector2 _direction;
		protected Vector2 _startPosition;
		protected float _speed = 30f;
		protected float _range = 7f;
		protected float _explosionX = 1;
		protected float _explosionY = 1;
		protected int _skillDamage;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		public virtual void Init(Vector2 direction, Vector2 startPosition, SkillExecutor executor,
			ParticleSystem projectileParticle,
			ParticleSystem explosionParticle = null)
		{
			SkillData skillData = executor.CurrentSkillData;
			PlayerData playerData = executor.Controller.Data;

			_direction = direction;
			_startPosition = startPosition + direction;
			_range = skillData.Range;
			_speed = skillData.ProjectileSpeed;
			_explosionX = skillData.DamageRangeX;
			_explosionY = skillData.DamageRangeY;

			// Damage
			_skillDamage = skillData.Damage + playerData.AtkDamage;
			if (Random.Range(0, 100) < playerData.CritRate)
			{
				_skillDamage *= (int)(playerData.CritDamage / 100.0f);
			}

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

			if (other.TryGetComponent(out Damageable e))
			{
				DestroyProjectile();
			}
		}

		protected virtual void DestroyProjectile()
		{
			// Physics
			ContactFilter2D contactFilter2D = ContactFilter2D.noFilter; // Todo: Set monster layer
			float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
			List<Collider2D> result = new();
			Physics2D.OverlapBox(transform.position, new Vector2(_explosionX, _explosionY), angle, contactFilter2D,
				result);

			foreach (Collider2D c in result)
			{
				if (!c.TryGetComponent(out Damageable enemy)) continue;

				enemy.TakeDamage(_skillDamage);
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
}