using UnityEngine;

public class DiceRollProjectile : MonoBehaviour, ISkillProjectile
{
	private Rigidbody2D _rb;
	private Vector2 _direction;
	private float _speed;
	private float _maxRange;
	private Vector2 _startPosition;

	private bool _isLaunched;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	public void Launch(SkillExecutor executor)
	{
		_direction = executor.Controller.FSM.FacingDir;
		_startPosition = executor.Controller.transform.position + new Vector3(_direction.x, _direction.y, 0);
		_speed = executor.CurrentSkillData.ProjectileSpeed;
		_maxRange = executor.CurrentSkillData.Range;

		transform.position = _startPosition;
		_isLaunched = true;
	}

	private void FixedUpdate()
	{
		if (!_isLaunched) return;

		Vector2 next = _rb.position + _direction * (_speed * Time.fixedDeltaTime);
		_rb.MovePosition(next);

		float traveled = Vector2.Distance(_startPosition, transform.position);
		if (traveled >= _maxRange)
		{
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!_isLaunched) return;
		// Destroy(gameObject); TODO: 충돌처리
	}
}