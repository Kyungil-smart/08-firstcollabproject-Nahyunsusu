using UnityEngine;

public class DiceRollProjectile : MonoBehaviour
{
	private Rigidbody2D _rb;
	private SkillData _data;
	
	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	public void Init(SkillData data, Vector2 dir)
	{
		transform.up = dir;
		_rb.linearVelocity = dir * data.ProjectileSpeed;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		Destroy(gameObject);
	}
}