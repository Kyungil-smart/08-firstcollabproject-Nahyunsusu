using System;
using UnityEngine;

public class TutorialDodger : MonoBehaviour
{
	public TutorialManager tutorialManager;

	public float rangeX;
	private bool _dir;
	private Vector3 _pos;

	private void Start()
	{
		_pos = gameObject.transform.position;
	}

	private void Update()
	{
		transform.Translate((_dir ? transform.right : Vector3.left) * (rangeX * Time.deltaTime));
		if (transform.position.x > _pos.x + rangeX) _dir = false;
		if (transform.position.x < _pos.x - rangeX) _dir = true;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController controller))
		{
			if (controller.IsInvincible)
			{
				tutorialManager.NotifyDashDodge();
			}
		}
	}
}