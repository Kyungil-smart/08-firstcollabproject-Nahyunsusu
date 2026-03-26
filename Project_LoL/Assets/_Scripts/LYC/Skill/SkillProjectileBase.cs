using UnityEngine;

public class SkillProjectileBase : MonoBehaviour
{
	private Rigidbody2D _rb;
	private ParticleSystem _projectile;
	private ParticleSystem _explosion;

	private float _speed = 30f;
	private float _range = 7f;
	private Vector2 _direction;
	private Vector2 _startPosition;

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
		

		float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
		// 히트 판정 처리
		DestroyProjectile();
	}

	private void DestroyProjectile()
	{
		// VFX를 부모에서 분리 후 파티클 잔상 재생
		Transform vfx = transform.Find("VFX");
		if (vfx != null)
		{
			vfx.SetParent(null);
			var ps = vfx.GetComponent<ParticleSystem>();
			if (ps != null)
			{
				ps.Stop();
				// 파티클이 모두 사라지면 제거
				Destroy(vfx.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
			}
		}

		Destroy(gameObject);
	}
}