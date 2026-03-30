using UnityEngine;

namespace _Scripts.LYC.Skill
{
	public class GigaRollProjectile : SkillProjectile
	{
		private float _extendedRange = 0;

		private void FixedUpdate()
		{
			_rb.MovePosition(_rb.position + _direction * (_speed * Time.fixedDeltaTime));
			float distance = Vector2.Distance(_startPosition, _rb.position);

			_extendedRange = (int)distance;
			if (_extendedRange >= 1)
			{
				transform.localScale = Vector3.one * _extendedRange;
				// TODO: Collider 크기 증가
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