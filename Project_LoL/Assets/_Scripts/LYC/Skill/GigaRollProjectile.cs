using UnityEngine;

namespace _Scripts.LYC.Skill
{
	public class GigaRollProjectile : SkillProjectile
	{
		private float _extendedRange = 0;
		private CircleCollider2D _collider;

		public override void Init(Vector2 direction, Vector2 startPosition, SkillExecutor executor, ParticleSystem projectileParticle,
			ParticleSystem explosionParticle = null)
		{
			_collider = GetComponent<CircleCollider2D>();
			base.Init(direction, startPosition, executor, projectileParticle, explosionParticle);
		}

		private void FixedUpdate()
		{
			_rb.MovePosition(_rb.position + _direction * (_speed * Time.fixedDeltaTime));
			float distance = Vector2.Distance(_startPosition, _rb.position);

			_extendedRange = (int)distance;
			if (_extendedRange >= 1)
			{
				transform.localScale = Vector3.one * _extendedRange;
				_collider.radius = _extendedRange * 0.5f;
			}

			if (Vector2.Distance(_startPosition, _rb.position) >= _range)
			{
				DestroyProjectile();
			}
		}

		protected override void DestroyProjectile()
		{
			_explosionX += _extendedRange;
			_explosionY += _extendedRange;
			base.DestroyProjectile();
		}
	}
}