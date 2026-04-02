using System.Collections;
using UnityEngine;

namespace _Scripts.LYC.Skill
{
	public class SkillHitbox : MonoBehaviour
	{
		protected int _damage;
		protected float _duration;
		protected ParticleSystem _effect;
		protected ParticleSystem _hitEffect;

		/// <summary>
		/// 근접 히트박스를 초기화합니다.
		/// <param name="direction"> direction이 Vector2.zero이면 방향이 없는 히트박스로 처리됩니다</param>
		/// <param name="position">히트박스의 실제 생성 위치는 <c>position + direction</c>입니다.</param>
		/// </summary>
		public void Init(Vector2 direction, Vector2 position, SkillExecutor executor, float duration,
			ParticleSystem effect, ParticleSystem hitEffect)
		{
			SkillData skillData = executor.CurrentSkillData;
			PlayerData playerData = executor.Controller.Data;
			_hitEffect = hitEffect;

			_damage = skillData.Damage + playerData.AtkDamage;
			_duration = duration;

			// Damage
			_damage = skillData.Damage + playerData.AtkDamage;
			if (Random.Range(0, 100) < playerData.CritRate)
			{
				_damage *= (int)(playerData.CritDamage / 100.0f);
			}

			// Root
			transform.right = direction;
			transform.position = position + direction;

			BoxCollider2D boxCollider = null;
			if (TryGetComponent(out boxCollider))
			{
				boxCollider.size = new Vector2(skillData.DamageRangeX, skillData.DamageRangeY);
				// 방향이 있을 때만 offset 적용: pivot을 끝으로 맞춤
				boxCollider.offset = new Vector2(direction == Vector2.zero ? 0 : skillData.DamageRangeX / 2.0f, 0);
			}

			if (effect)
			{
				_effect = Instantiate(effect, transform);
				// 방향이 있을 때만 이펙트 offset 적용
				if (direction != Vector2.zero)
					_effect.transform.SetLocalPositionAndRotation(Vector3.right * skillData.DamageRangeX / 2.0f, Quaternion.identity);
				_effect.transform.localScale = new Vector3(skillData.DamageRangeX, skillData.DamageRangeY, 1);
				var main = _effect.main;
				main.duration = _duration;
				main.startLifetime = _duration;
				_effect.Play();
			}

			if (boxCollider != null)
				CheckInitialOverlap(boxCollider);

			StartCoroutine(DestroyAfterDuration());
		}

		private void CheckInitialOverlap(BoxCollider2D boxCollider)
		{
			var filter = new ContactFilter2D { useTriggers = true };
			var results = new Collider2D[16];
			int count = boxCollider.Overlap(filter, results);
			for (int i = 0; i < count; i++)
				ProcessHit(results[i]);
		}

		private void OnTriggerEnter2D(Collider2D other) => ProcessHit(other);

		private void ProcessHit(Collider2D other)
		{
			if (other.TryGetComponent(out Damageable enemy))
			{
				if (enemy is PlayerController) return;
				enemy.TakeDamage(_damage);

				var ps = Instantiate(_hitEffect, other.ClosestPoint(transform.position), other.transform.rotation);
				ps.Play();
				Destroy(ps.gameObject, ps.main.duration);
			}
		}

		protected virtual IEnumerator DestroyAfterDuration()
		{
			yield return new WaitForSeconds(_duration);
			DestroyHitbox();
		}

		protected void DestroyHitbox()
		{
			if (_effect != null)
			{
				_effect.transform.SetParent(null);
				Destroy(_effect.gameObject, _effect.main.duration);
			}

			Destroy(gameObject);
		}
	}
}